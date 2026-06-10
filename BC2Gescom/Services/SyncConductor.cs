using BC2Gescom.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace BC2Gescom.Services;

public class SyncConductor
{
    private readonly BCDataAccess _bcData;
    private readonly GescomDataAccess _gescomData;

    private readonly BCTableNames _bcTableNames;

    //BC data stored in memory to reduce database calls and more controlled iteration
    private Dictionary<string, string> _bcCountryMap = new();
    private Dictionary<string, Model.BC.Invoice> _bcInvoiceLookup = new();
    private IEnumerable<Model.BC.Invoice>? _bcInvoices;
    private IEnumerable<Model.BC.CreditMemo>? _bcCreditMemos;
    private IEnumerable<Model.BC.LineItem>? _bcInvoiceLines;
    private IEnumerable<Model.BC.LineItem>? _bcCreditLines;
    private IEnumerable<Model.BC.Customer>? _bcCustomers;
    private IEnumerable<Model.Transient.Address>? _bcCustomerAddresses;


    //Gescom IDs stored in memory for more controlled retrieval
    private Dictionary<string, int> _gescomCountryIds = new();
    private Dictionary<string, int> _gescomAddressIds = new();
    private Dictionary<string, int> _gescomAccountIds = new();
    private Dictionary<string, int> _gescomCustomerIds = new();
    private Dictionary<string, int> _gescomOrderIdsByOrderNumber = new();



    public SyncConductor(BCDataAccess bcData, GescomDataAccess gescomData, IOptions<BCTableNames> tableNames)
    {
        _bcData = bcData;
        _gescomData = gescomData;
        _bcTableNames = tableNames.Value;
    }

    private async Task LoadBCData()
    {
        Trace.WriteLine("Loading BC Data...");

        _bcCountryMap = (await _bcData.GetCountryRegionsAsync()).ToDictionary(x => x.Abbreviation, x => x.DisplayName);         
        _bcInvoices = await _bcData.GetInvoicesAsync();                                                                         
        _bcInvoiceLookup = _bcInvoices.ToDictionary(x => x.InvoiceBCID, x => x); 
        _bcInvoiceLines = await _bcData.GetInvoiceLinesAsync();                                                                 
        _bcCreditMemos = await _bcData.GetCreditMemosAsync();                                                                   
        _bcCreditLines = await _bcData.GetCreditLinesAsync();                                                                   
        _bcCustomers = await _bcData.GetRelevantCustomersAsync();                                                               
        _bcCustomerAddresses = await _bcData.GetRelevantCustomerAddressesAsync();
        
        Trace.WriteLine("BC data loaded");
    }

    public async Task Run()
    {
        await LoadBCData();

        string[] missingPartNumbers = await ComparePartNumbers();

        if (missingPartNumbers.Length != 0)
        {
            Trace.WriteLine("Found parts in BC that don't exist in Gescom. Ammend the following parts then run the sync again:");
            Trace.WriteLine($"{string.Join(", ", missingPartNumbers)}");
            Console.WriteLine();
            Trace.WriteLine("Sync cancelled");
            return;
        }

        //addresses, accounts, customers, and customer types from bc_Customers table
        await ResolveCustomerData();

        //orders, sales orders, invoices
        await ResolveSalesData();
    }

    #region Customer

    private async Task ResolveCustomerData()
    {
        Trace.WriteLine("Begin resolving customer data in Gescom");

        await UpdateRelevantAddresses();

        await UpdateRelevantAccounts();

        await UpdateRelevantCustomers();
    }

    private bool IsGescomCustomerIncomplete(Model.GescomEU.Customer gescomCustomer, Model.BC.Customer bcCustomer)
    {
        int correctAddressId = _gescomAddressIds[bcCustomer.StringifyAddress()];
        int correctAccountId = _gescomAccountIds[bcCustomer.CustomerBCID];

        return (gescomCustomer.BillToAddress_InstanceId != correctAddressId || gescomCustomer.BillToAddress_TypeId != GescomType.Address
                 || gescomCustomer.BCID != bcCustomer.CustomerBCID
                 || gescomCustomer.AccountingCode != bcCustomer.CustomerNumber
                 || gescomCustomer.Account_InstanceId != correctAccountId
                 || gescomCustomer.AccountingName != bcCustomer.CustomerName
                 || gescomCustomer.CustomerType_InstanceId != GescomConstant.GetCustomerType(bcCustomer) || gescomCustomer.CustomerType_TypeId != GescomType.CustomerType);
    }

    private async Task UpdateRelevantAddresses()
    {
        int addressCount = 1;
        int addressTotal = _bcCustomerAddresses!.Count();

        Trace.WriteLine("Begin resolving relevant addresses in Gescom");
        Trace.WriteLine($"Creating/updating {addressTotal} addresses");
        //addresses
        foreach (var customerAddress in _bcCustomerAddresses!)
        {
            Trace.WriteLine($"Resolving address {addressCount} of {addressTotal}");
            addressCount++;
            
            if (!_bcCountryMap.ContainsKey(customerAddress.CountryAbbr))
            {
                //we have problems
                Trace.WriteLine($"Critical error: Country abbreviation '{customerAddress.CountryAbbr}' not found in BC Country Name/Abbreviation mapping table.");
                Trace.WriteLine($"Close application, update {_bcTableNames.CountryRegion} table in BC database, then run the application again");
                Console.ReadKey();
                //need to refactor on how to cancel, but this is good enough for now
            }


            string countryName = _bcCountryMap[customerAddress.CountryAbbr];

            int countryId;

            if (_gescomCountryIds.ContainsKey(countryName))
            {
                countryId = _gescomCountryIds[countryName];
            }
            else
            {
                countryId = await _gescomData.GetCountryIdAsync(countryName);
            }

            if (countryId == 0)
            {
                Trace.WriteLine("");
                countryId = await _gescomData.CreateCountryReturnIdAsync(countryName);
            }

            //add country id to repo
            _gescomCountryIds[countryName] = countryId;

            string addressString = customerAddress.Stringify();
            int addressId;

            if (_gescomAddressIds.ContainsKey(addressString))
            {
                //if address already exists, no further action needed
                continue;
            }
            else
            {
                //try to fetch it
                addressId = await _gescomData.GetAddressIdAsync(customerAddress, countryId);
            }


            if (addressId == 0)
            {
                addressId = await _gescomData.CreateAddressReturnIdAsync(customerAddress, countryId);
            }

            _gescomAddressIds[addressString] = addressId;

            //init just in case
            countryId = 0;
            addressId = 0;
            
        }
    }

    private async Task UpdateRelevantAccounts()
    {
        int accountCountTotal = _bcCustomers!.Count();
        int accountCountCurrent = 1;

        foreach (Model.BC.Customer bcCustomer in _bcCustomers!)
        {

            Console.WriteLine($"Processing account {accountCountCurrent} of {accountCountTotal}");
            accountCountCurrent++;
            //first try to find account based on BCID
            Model.GescomEU.Account[] matchingAccountsByBCID = (await _gescomData.GetAccountByBCIDAsync(bcCustomer)).ToArray();

            //if found, use the BCID as the key and store the instanceid -- there should only be ONE here if i do everything right
            if (matchingAccountsByBCID.Length == 1)
            {
                _gescomAccountIds[bcCustomer.CustomerBCID] = matchingAccountsByBCID[0].InstanceId;
                continue;
            }

            //getting account by name here instead of BCID so that we can add BCIDs as we go
            Model.GescomEU.Account[] matchingAccountsByName = (await _gescomData.GetAccountByNameAsync(bcCustomer)).ToArray();

            bool isAllAccountsHaveBCID = matchingAccountsByName.Where(x => x.BCID is not null).Count() == matchingAccountsByName.Length;

            if (matchingAccountsByName.Length == 0 || isAllAccountsHaveBCID)
            {
                //Case 1: this account doesnt exist yet, create new customer with correct address
                //Case 2: this is another distinct customer within BC, need to create a new account in gescom
                string addressString = bcCustomer.StringifyAddress();
                int addressId = _gescomAddressIds[addressString];
                var newAccountId = await _gescomData.CreateAccountReturnIdAsync(bcCustomer, addressId);
                _gescomAccountIds[bcCustomer.CustomerBCID] = newAccountId;
                continue;
                //added account to repo, move on to next account
            }

                //there are one or many matching accounts, find the most correct one, IF NOT then make sure to update to be correct
                Model.GescomEU.Account? completeAccount = null;
                int correctAccountAddressId = _gescomAddressIds[bcCustomer.StringifyAddress()];

            foreach (var account in matchingAccountsByName)
            {
                //check all cases where gescom account might have bad/missing information
                if (!IsGescomAccountIncomplete(account, bcCustomer))
                {
                    completeAccount = account;
                    break;
                }
            }

            if (completeAccount is null)
            {
                //no account is complete, update the first one found WITHOUT A BCID to be accurate
                completeAccount = matchingAccountsByName.Where(x => x.BCID is null).First();
                int targetAccountId = completeAccount.InstanceId;
                await _gescomData.UpdateAccountMatchBCAsync(targetAccountId, correctAccountAddressId, bcCustomer);
            }

            _gescomAccountIds[bcCustomer.CustomerBCID] = completeAccount.InstanceId;



        }
    }

    private async Task UpdateRelevantCustomers()
    {
        int customerCountCurrent = 1;
        int customerCountTotal = _bcCustomers!.Count();

        foreach (var bcCustomer in _bcCustomers!)
        { 
            Console.WriteLine($"Processing customer {customerCountCurrent} of {customerCountTotal}");
            customerCountCurrent++;

            //first try to find customer based on BCID
            Model.GescomEU.Customer[] matchingCustomersByBCID = (await _gescomData.GetCustomerByBCIDAsync(bcCustomer)).ToArray();

            //if found, use the BCID as the key and store the instanceid -- there should only be ONE here if i do everything right
            if (matchingCustomersByBCID.Length == 1)
            {
                _gescomCustomerIds[bcCustomer.CustomerBCID] = matchingCustomersByBCID[0].InstanceId;
                continue;
            }

            //getting customer by AccountName here instead of BCID so that we can add BCIDs as we go
            Model.GescomEU.Customer[] matchingCustomersByName = (await _gescomData.GetCustomerByNameAsync(bcCustomer)).ToArray();

            bool isAllCustomersHaveBCID = matchingCustomersByName.Where(x => x.BCID is not null).Count() == matchingCustomersByName.Length;

            if (matchingCustomersByName.Length == 0 || isAllCustomersHaveBCID)
            {
                //Case 1: this account doesnt exist yet, create new customer with correct address
                //Case 2: this is another distinct customer within BC, need to create a new customer in gescom
                string addressString = bcCustomer.StringifyAddress();
                int addressId = _gescomAddressIds[addressString];
                int accountId = _gescomAccountIds[bcCustomer.CustomerBCID];
                int newCustomerId = await _gescomData.CreateCustomerReturnIdAsync(bcCustomer, accountId, addressId);
                _gescomCustomerIds[bcCustomer.CustomerBCID] = newCustomerId;
                continue;
                //added to customer repo, move on to next
            }

            //there are one or many matching accounts, find the most correct one, IF NOT then make sure to update to be correct
            Model.GescomEU.Customer? completeCustomer = null;
            int correctAddressId = _gescomAddressIds[bcCustomer.StringifyAddress()];

            foreach (var gescomCustomer in matchingCustomersByName)
            {
                //check all cases where gescom account might have bad/missing information
                if (!IsGescomCustomerIncomplete(gescomCustomer, bcCustomer))
                {
                    completeCustomer = gescomCustomer;
                    break;
                }
            }

            if (completeCustomer is null)
            {
                //no customer in gescom is complete, pick the first one WITHOUT A BCID and update it 
                completeCustomer = matchingCustomersByName.Where(x => x.BCID is null).First();
                int targetCustomerId = completeCustomer.InstanceId;
                int correctAccountId = _gescomAccountIds[bcCustomer.CustomerBCID];

                await _gescomData.UpdateCustomerMatchBCAsync(bcCustomer, correctAccountId, correctAddressId, completeCustomer.InstanceId);
            }

            _gescomCustomerIds[bcCustomer.CustomerBCID] = completeCustomer.InstanceId;

            
        }
    }

    private bool IsGescomAccountIncomplete(Model.GescomEU.Account gescomAccount, Model.BC.Customer bcCustomer)
    {
        int correctAccountAddressId = _gescomAddressIds[bcCustomer.StringifyAddress()];

        return (gescomAccount.Address_InstanceId is null || gescomAccount.Address_InstanceId.Equals(GescomConstant.UnknownAddressId) || gescomAccount.Address_InstanceId != correctAccountAddressId
                        || gescomAccount.Code is null || gescomAccount.Code == string.Empty || gescomAccount.Code != bcCustomer.CustomerNumber
                        || gescomAccount.BCID is null || gescomAccount.BCID != bcCustomer.CustomerBCID);
    }

    #endregion Customer

    #region InvoiceCredit

    private async Task ResolveSalesData()
    {
        await ResolveInvoices();

        await ResolveCredits();

        await ResolveLines();
    }

    private async Task ResolveInvoices()
    {
        int invoiceCountCurrent = 1;
        int invoiceCountTotal = _bcInvoices!.Count();

        foreach (var bcInvoice in _bcInvoices!)
        {
            Console.WriteLine($"Processing invoice {invoiceCountCurrent} of {invoiceCountTotal}");
            invoiceCountCurrent++;


            IEnumerable<Model.GescomEU.SalesOrder>? matchingSalesOrders = null;
            //TESTING
            if (bcInvoice.OrderNumber == "S615347583")
            {

            }
            //TESTING

            Model.GescomEU.Customer gescomCustomer = (await _gescomData.GetCustomerByBCIDAsync(bcInvoice.BillToCustomerId)).First();

            int gescomOrderId;

            if (_gescomOrderIdsByOrderNumber.ContainsKey(bcInvoice.OrderNumber))
            {
                gescomOrderId = _gescomOrderIdsByOrderNumber[bcInvoice.OrderNumber];
            }
            else if ((matchingSalesOrders = await _gescomData.GetSalesOrderByBCInvoice(bcInvoice)).Any())
            {
                gescomOrderId = matchingSalesOrders.First().InstanceId;
            }
            else
            {
                gescomOrderId = await _gescomData.CreateOrderReturnIdAsync(bcInvoice, gescomCustomer);
            }


            //  !!   Now irrelevant with above logic implemented   !!
            //if (!_gescomOrderIdsByOrderNumber.ContainsKey(bcInvoice.OrderNumber))
            //{
            //    await _gescomData.CreateSalesOrderReturnIdAsync(bcInvoice, gescomOrderId);
            //}
            
            await _gescomData.CreateInvoiceReturnIdAsync(bcInvoice, gescomOrderId);

            _gescomOrderIdsByOrderNumber[bcInvoice.OrderNumber] = gescomOrderId;

        }
    }


    private async Task ResolveCredits()
    {
        int creditCountCurrent = 1;
        int creditCountTotal = _bcCreditMemos!.Count();

        foreach (var bcCredit in _bcCreditMemos!)
        {
            Console.WriteLine($"Processing credit {creditCountCurrent} of {creditCountTotal}");
            creditCountCurrent++;

            Model.GescomEU.Customer gescomCustomer = (await _gescomData.GetCustomerByBCIDAsync(bcCredit.BillToCustomerId)).First();

            //order

            int gescomOrderId = await _gescomData.CreateCreditOrderReturnIdAsync(bcCredit, gescomCustomer);

            //sales order
            await _gescomData.CreateCreditSalesOrderAsync(bcCredit, gescomOrderId);


            //delivery
            await _gescomData.CreateCreditDeliveryAsync(bcCredit, gescomOrderId);

            //credit
            await _gescomData.CreateCreditAsync(bcCredit, gescomOrderId);

        }
    }

    private async Task ResolveLines()
    {
        await _gescomData.InsertOrderLinesAsync();
    }


    #endregion InvoiceCredit

    private async Task<string[]> ComparePartNumbers()
    {
        var bcPartNumbers = await _bcData.GetDistinctPartNumbers();
        var gescomPartNumbers = await _gescomData.GetDistinctPartNumbersAsync();

        return bcPartNumbers.Except(gescomPartNumbers).ToArray();
    }

   
}

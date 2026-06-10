using BC2Gescom.Configuration;
using BC2Gescom.Model.BC;
using BC2Gescom.Model.GescomEU;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;

namespace BC2Gescom.Services;

public class GescomDataAccess : DataAccess
{
    private readonly GescomTableNames _tableNames;

    public GescomDataAccess(IOptions<ConnectionStrings> appSettings, IOptions<GescomTableNames> tableNames) : base(appSettings)
    {
        _connectionString = _appSettings.GescomConnectionString;
        _tableNames = tableNames.Value;
    }

    public async Task<IEnumerable<string>> GetDistinctPartNumbersAsync()
    {
        string query = $"SELECT DISTINCT [Code] FROM {_tableNames.Product} ORDER BY [Code]";

        return await GetDbResultAsync<string>(query);
    }


    public async Task<int> GetCountryIdAsync(string countryName)
    {
        string query = $"SELECT TOP 1 * FROM {_tableNames.Country} WHERE [Name] = '{countryName}' ORDER BY InstanceID";

        var matchingCountries = (await GetDbResultAsync<Model.GescomEU.Country>(query)).ToArray();

        if (matchingCountries.Length == 0)
        {
            return 0;
        }

        return matchingCountries[0].InstanceId;

    }

    public async Task<int> GetAddressIdAsync(Model.Transient.Address targetAddress, int countryId)
    {
        string query = $@"SELECT TOP 1 InstanceID
FROM {_tableNames.Address}
WHERE Country_InstanceID = {countryId}
AND Street = @AddressLine1
AND StreetExtend = @AddressLine2
AND City = @City
AND Region = @State
AND ZipCode = @PostalCode";

        int[] matchingAddresses = (await GetDbResultWithParameterAsync<int, Model.Transient.Address>(query, targetAddress)).ToArray();

        if (matchingAddresses.Length == 0)
        {
            return 0;
        }
        else
        {
            return matchingAddresses[0];
        }
    }

    public async Task<int> CreateCountryReturnIdAsync(string countryName)
    {
        string insertQuery = $@"INSERT INTO {_tableNames.Country}(
InstanceID
,TypeID
,Branch_InstanceID
,Branch_TypeID
,Market_InstanceID
,Market_TypeID
,[Name]
)
VALUES
(
NEXT VALUE FOR InstanceId
,{GescomType.Country}
,{GescomConstant.BranchID}
,{GescomType.Branch}
,{GescomConstant.MarketID}
,{GescomType.Market}
,'{countryName}'
)";

        await RunQueryAsync(insertQuery);

        int countryId = await GetCountryIdAsync(countryName);

        if (countryId == 0)
        {
            //we have problems, throw an error
            throw new Exception("Created country but couldn't get ID, we have problems");
        }

        return countryId;
    }

    public async Task<int> CreateAddressReturnIdAsync(Model.Transient.Address address, int countryId)
    {

        string insertQuery = $@"INSERT INTO [GescomEU_merge].[dbo].[Address] 
            (
               [InstanceId]
              ,[TypeId]
              ,[Country_InstanceId]
              ,[Country_TypeId]
              ,[Region]
              ,[ZipCode]
              ,[City]
              ,[StreetExtend]
              ,[Street]
            )
            VALUES
            (
                NEXT VALUE FOR [InstanceId],
                {GescomType.Address},
                {countryId},
                {GescomType.Country},
                @State,
                @PostalCode,
                @City,
                @AddressLine2,
                @AddressLine1
            )";

        await RunQueryWithParameterAsync(insertQuery, address);

        int addressId = await GetAddressIdAsync(address, countryId);

        if (addressId == 0)
        {
            //we have problems throw error
            throw new Exception("Inserted address but couldn't find id, we have problems");
        }

        return addressId;
    }



    public async Task<IEnumerable<Model.GescomEU.Account>> GetAccountByNameAsync(Model.BC.Customer bcCustomer)
    {
        string query = $"select * from {_tableNames.Account} where [Name] = @CustomerName";

        return await GetDbResultWithParameterAsync<Model.GescomEU.Account, Model.BC.Customer>(query, bcCustomer);
    }

    public async Task<IEnumerable<Model.GescomEU.Account>> GetAccountByBCIDAsync(Model.BC.Customer bcCustomer)
    {
        string query = $"select * from {_tableNames.Account} where [BCID] = @CustomerBCID";

        return await GetDbResultWithParameterAsync<Model.GescomEU.Account, Model.BC.Customer>(query, bcCustomer);
    }

    public async Task<int> CreateAccountReturnIdAsync(Model.BC.Customer bcCustomer, int addressId)
    {
        string query = $@"INSERT INTO [GescomEU_merge].[dbo].[Account](
                                [InstanceId]
                              ,[TypeId]
                              ,[Branch_InstanceId]
                              ,[Branch_TypeId]
                              ,[Address_InstanceId]
                              ,[Address_TypeId]
                              ,[Language_InstanceId]
                              ,[Language_TypeId]
                              ,[AccountType]
                              ,[DefaultCarrier_InstanceId]
                              ,[DefaultCarrier_TypeId]
                              ,[SmacCode]
                              ,[ShippingAct]
                              ,[IsObsoleted]
                              ,[Code]
                              ,[EmailTo]
                              ,[Web]
                              ,[OrderingNotes]
                              ,[Phone]
                              ,[Name] 
                              ,[BCID])
                              VALUES
                              (
                              NEXT VALUE FOR [InstanceId]
                              ,{GescomType.Account}
                              ,{GescomConstant.BranchID}
                              ,{GescomType.Branch}
                              ,{addressId}
                              ,{GescomType.Address}
                              ,{GescomConstant.LanguageID}
                              ,{GescomType.Language}
                              ,0
                              ,NULL
                              ,{GescomType.Carrier}
                              ,NULL
                              ,NULL
                              ,0
                              ,@CustomerNumber
                              ,@Email
                              ,NULL
                              ,'Created from BC'
                              ,@PhoneNumber
                              ,@CustomerName
                              ,@CustomerBCID
                              )";

        await RunQueryWithParameterAsync(query, bcCustomer);

        var account = (await GetAccountByBCIDAsync(bcCustomer)).ToArray()[0];

        return account.InstanceId;
    }

    public async Task UpdateAccountMatchBCAsync(int accountId, int addressId, Model.BC.Customer bcCustomer)
    {
        string query = $@"update {_tableNames.Account}
                            SET Address_InstanceId = {addressId}, [Code] = @CustomerNumber, Phone = @PhoneNumber, [Name] = @CustomerName, BCID = @CustomerBCID
                            WHERE InstanceId = {accountId}";

        await RunQueryWithParameterAsync(query, bcCustomer);
    }



    public async Task<IEnumerable<Model.GescomEU.Customer>> GetCustomerByNameAsync(Model.BC.Customer bcCustomer)
    {
        string query = $"select * FROM {_tableNames.Customer} WHERE AccountingName = @CustomerName";

        return await GetDbResultWithParameterAsync<Model.GescomEU.Customer, Model.BC.Customer>(query, bcCustomer);
    }

	public async Task<IEnumerable<Model.GescomEU.Customer>> GetCustomerByBCIDAsync(Model.BC.Customer bcCustomer) => await GetCustomerByBCIDAsync(bcCustomer.CustomerBCID);


    public async Task<IEnumerable<Model.GescomEU.Customer>> GetCustomerByBCIDAsync(string bcid)
	{
		string query = $"select * from {_tableNames.Customer} WHERE BCID = '{bcid}'";

		return await GetDbResultAsync<Model.GescomEU.Customer>(query);
	}

    public async Task<int> CreateCustomerReturnIdAsync(Model.BC.Customer bcCustomer, int gescomAccountId, int gescomAddressId)
    {
        int customerTypeInstanceId = GescomConstant.GetCustomerType(bcCustomer);

        string query = $@"INSERT INTO {_tableNames.Customer}
				( [InstanceId]
				  ,[TypeId]
				  ,[SalesPerson_InstanceId]
				  ,[SalesPerson_TypeId]
				  ,[AccountingDepartment_InstanceId]
				  ,[AccountingDepartment_TypeId]
				  ,[CustomerType_InstanceId]
				  ,[CustomerType_TypeId]
				  ,[Currency_InstanceId]
				  ,[Currency_TypeId]
				  ,[BlockedBy_InstanceId]
				  ,[BlockedBy_TypeId]
				  ,[CreatedBy_InstanceId]
				  ,[CreatedBy_TypeId]
				  ,[Term_InstanceId]
				  ,[Term_TypeId]
				  ,[PaymentMethod_InstanceId]
				  ,[PaymentMethod_TypeId]
				  ,[InvoiceMethod_InstanceId]
				  ,[InvoiceMethod_TypeId]
				  ,[Account_InstanceId]
				  ,[Account_TypeId]
				  ,[BillToAddress_InstanceId]
				  ,[BillToAddress_TypeId]
				  ,[SMAFactor]
				  ,[TermTolerance]
				  ,[TermPeriod]
				  ,[AccountingCode]
				  ,[AccountingName]
				  ,[CreatedOn]
				  ,[SalesOrderEmailTo]
				  ,[RenewalEmailTo]
				  ,[InvoiceEmailTo]
				  ,[CreditLimit]
				  ,[BlockedOn]
				  ,[Balance]
				  ,[AccountingNotes]
                  ,[BCID])
				 VALUES
				  (
					  NEXT VALUE FOR [InstanceId]
					,{GescomType.Customer}
					,NULL
					,{GescomType.SalesPerson}
					,{GescomConstant.AccountingDepartmentID}
					,{GescomType.AccountingDepartment}
					,{customerTypeInstanceId}
					,{GescomType.CustomerType}
					,{GescomConstant.CurrencyID}
					,{GescomType.Currency}
					,NULL
					,{GescomType.User}
					,NULL
					,{GescomType.User}
					,NULL
					,{GescomType.Term}
					,NULL
					,{GescomType.PaymentMethod}
					,NULL
					,{GescomType.InvoiceMethod}
					,{gescomAccountId}
					,{GescomType.Account}
					,{gescomAddressId}
					,{GescomType.Address}
					,0
					,5
					,NULL
					,@CustomerNumber
					,@CustomerName
					,GETDATE()
					,NULL
					,NULL
					,NULL
					,0
					,'0001-01-01 00:00:00.0000000'
					,0
					,NULL
                    ,@CustomerBCID
				  )";

        await RunQueryWithParameterAsync(query, bcCustomer);

        var customer = (await GetCustomerByBCIDAsync(bcCustomer)).First();

        return customer.InstanceId;
    }

    public async Task UpdateCustomerMatchBCAsync(Model.BC.Customer bcCustomer, int gescomAccountId, int gescomAddressId, int gescomCustomerId)
    {
        int customerTypeInstanceId = GescomConstant.GetCustomerType(bcCustomer);

        string query = $@"UPDATE {_tableNames.Customer}
SET BillToAddress_InstanceId = {gescomAddressId}
,BillToAddress_TypeId = {GescomType.Address}
,BCID = @CustomerBCID
,AccountingCode = @CustomerNumber
,Account_InstanceId = {gescomAccountId}
,Account_TypeId = {GescomType.Account}
,AccountingName = @CustomerName
,CustomerType_InstanceId = {customerTypeInstanceId}
,CustomerType_TypeId = {GescomType.CustomerType}
WHERE InstanceId = {gescomCustomerId}";

        await RunQueryWithParameterAsync(query, bcCustomer);
    }



    public async Task<IEnumerable<Model.GescomEU.Order>> GetOrderByBCIDAsync(string bcid)
    {
        string query = $"SELECT * FROM {_tableNames.Order} WHERE BCID = '{bcid}'";

        return await GetDbResultAsync<Model.GescomEU.Order>(query);
    }

    public async Task<IEnumerable<Model.GescomEU.Order>> GetOrderByBCIDAsync(Model.BC.Invoice bcInvoice) => await GetOrderByBCIDAsync(bcInvoice.OrderId);

	public async Task<IEnumerable<Model.GescomEU.Order>> GetOrderByUntrackedCreditAsync(Model.BC.CreditMemo bcCredit) => await GetOrderByBCIDAsync(bcCredit.CreditMemoBCID);

    public async Task<int> CreateOrderReturnIdAsync(Model.BC.Invoice bcInvoice, Model.GescomEU.Customer gescomCustomer)
    {
        string query = $@"INSERT INTO [GescomEU_merge].[ids].[Order_tbl]
				( [InstanceId]
				  ,[TypeId]
				  ,[Branch_InstanceId]
				  ,[Branch_TypeId]
				  ,[OrderType_InstanceId]
				  ,[OrderType_TypeId]
				  ,[SupportPerson_InstanceId]
				  ,[SupportPerson_TypeId]
				  ,[ShipToAddress_InstanceId]
				  ,[ShipToAddress_TypeId]
				  ,[BillToAddress_InstanceId]
				  ,[BillToAddress_TypeId]
				  ,[Carrier_InstanceId]
				  ,[Carrier_TypeId]
				  ,[ShipTo_InstanceId]
				  ,[ShipTo_TypeId]
				  ,[DeliveryType_InstanceId]
				  ,[DeliveryType_TypeId]
				  ,[Currency_InstanceId]
				  ,[Currency_TypeId]
				  ,[BillTo_InstanceId]
				  ,[BillTo_TypeId]
				  ,[SalesPerson_InstanceId]
				  ,[SalesPerson_TypeId]
				  ,[BillToContact_InstanceId]
				  ,[BillToContact_TypeId]
				  ,[ShipToContact_InstanceId]
				  ,[ShipToContact_TypeId]
				  ,[PO]
				  ,[Tax]
				  ,[EndUserEmailTo]
				  ,[Total]
				  ,[Subtotal]
				  ,[EmailTo]
				  ,[PO2]
				  ,[Notes]
                  ,[BCID]
				  ,[SystemModifiedOn])
				  VALUES
				  (
					 NEXT VALUE FOR [InstanceId]
					,{GescomType.Order}
					,{GescomConstant.BranchID}
					,{GescomType.Branch}
					,{GescomConstant.OrderTypeID}
					,{GescomType.OrderType}
					,NULL
					,{GescomType.SupportPerson}
					,{gescomCustomer.BillToAddress_InstanceId}
					,{GescomType.Address}
					,{gescomCustomer.BillToAddress_InstanceId}
					,{GescomType.Address}
					,NULL
					,{GescomType.Carrier}
					,{gescomCustomer.Account_InstanceId}
					,{GescomType.Account}
					,{GescomConstant.DeliveryTypeElectronic}
					,{GescomType.DeliveryType}
					,{GescomConstant.CurrencyID}
					,{GescomType.Currency}
					,{gescomCustomer.InstanceId}
					,{GescomType.Customer}
					,NULL
					,{GescomType.SalesPerson}
					,NULL
					,{GescomType.Contact}
					,NULL
					,{GescomType.Contact}
					,'{bcInvoice.PONumber}'
					,0
					,NULL
					,{bcInvoice.TotalAmountExcludingTax}
					,{bcInvoice.TotalAmountExcludingTax}
					,NULL
					,'{bcInvoice.CustomerPurchaseOrderReference}'
					,NULL
                    ,'{bcInvoice.OrderId}'
					,'{bcInvoice.PostingDate}'
				  )";

        await RunQueryAsync(query);

        return (await GetOrderByBCIDAsync(bcInvoice)).First().InstanceId;
    }

	public async Task<int> CreateOrderReturnIdAsync(Model.BC.CreditMemo bcCredit, Model.GescomEU.Customer gescomCustomer)
	{
        string query = $@"INSERT INTO [GescomEU_merge].[ids].[Order_tbl]
				( [InstanceId]
				  ,[TypeId]
				  ,[Branch_InstanceId]
				  ,[Branch_TypeId]
				  ,[OrderType_InstanceId]
				  ,[OrderType_TypeId]
				  ,[SupportPerson_InstanceId]
				  ,[SupportPerson_TypeId]
				  ,[ShipToAddress_InstanceId]
				  ,[ShipToAddress_TypeId]
				  ,[BillToAddress_InstanceId]
				  ,[BillToAddress_TypeId]
				  ,[Carrier_InstanceId]
				  ,[Carrier_TypeId]
				  ,[ShipTo_InstanceId]
				  ,[ShipTo_TypeId]
				  ,[DeliveryType_InstanceId]
				  ,[DeliveryType_TypeId]
				  ,[Currency_InstanceId]
				  ,[Currency_TypeId]
				  ,[BillTo_InstanceId]
				  ,[BillTo_TypeId]
				  ,[SalesPerson_InstanceId]
				  ,[SalesPerson_TypeId]
				  ,[BillToContact_InstanceId]
				  ,[BillToContact_TypeId]
				  ,[ShipToContact_InstanceId]
				  ,[ShipToContact_TypeId]
				  ,[PO]
				  ,[Tax]
				  ,[EndUserEmailTo]
				  ,[Total]
				  ,[Subtotal]
				  ,[EmailTo]
				  ,[PO2]
				  ,[Notes]
                  ,[BCID]
				  ,[SystemModifiedOn])
				  VALUES
				  (
					 NEXT VALUE FOR [InstanceId]
					,{GescomType.Order}
					,{GescomConstant.BranchID}
					,{GescomType.Branch}
					,{GescomConstant.OrderTypeID}
					,{GescomType.OrderType}
					,NULL
					,{GescomType.SupportPerson}
					,{gescomCustomer.BillToAddress_InstanceId}
					,{GescomType.Address}
					,{gescomCustomer.BillToAddress_InstanceId}
					,{GescomType.Address}
					,NULL
					,{GescomType.Carrier}
					,{gescomCustomer.Account_InstanceId}
					,{GescomType.Account}
					,{GescomConstant.DeliveryTypeElectronic}
					,{GescomType.DeliveryType}
					,{GescomConstant.CurrencyID}
					,{GescomType.Currency}
					,{gescomCustomer.InstanceId}
					,{GescomType.Customer}
					,NULL
					,{GescomType.SalesPerson}
					,NULL
					,{GescomType.Contact}
					,NULL
					,{GescomType.Contact}
					,@PONumber
					,0
					,NULL
					,@TotalAmountExcludingTax
					,@TotalAmountExcludingTax
					,NULL
					,@PONumber
					,NULL
                    ,@CreditMemoBCID --USING CREDIT BCID HERE BECAUSE THE ORDER DOES NOT EXIST
					,@PostingDate
				  )";

        await RunQueryWithParameterAsync<Model.BC.CreditMemo>(query, bcCredit);

        return (await GetOrderByUntrackedCreditAsync(bcCredit)).First().InstanceId;
    }

	public async Task<int> CreateCreditOrderReturnIdAsync(Model.BC.CreditMemo bcCredit, Model.GescomEU.Customer gescomCustomer)
	{
        string query = $@"INSERT INTO [GescomEU_merge].[ids].[Order_tbl]
				( [InstanceId]
				  ,[TypeId]
				  ,[Branch_InstanceId]
				  ,[Branch_TypeId]
				  ,[OrderType_InstanceId]
				  ,[OrderType_TypeId]
				  ,[SupportPerson_InstanceId]
				  ,[SupportPerson_TypeId]
				  ,[ShipToAddress_InstanceId]
				  ,[ShipToAddress_TypeId]
				  ,[BillToAddress_InstanceId]
				  ,[BillToAddress_TypeId]
				  ,[Carrier_InstanceId]
				  ,[Carrier_TypeId]
				  ,[ShipTo_InstanceId]
				  ,[ShipTo_TypeId]
				  ,[DeliveryType_InstanceId]
				  ,[DeliveryType_TypeId]
				  ,[Currency_InstanceId]
				  ,[Currency_TypeId]
				  ,[BillTo_InstanceId]
				  ,[BillTo_TypeId]
				  ,[SalesPerson_InstanceId]
				  ,[SalesPerson_TypeId]
				  ,[BillToContact_InstanceId]
				  ,[BillToContact_TypeId]
				  ,[ShipToContact_InstanceId]
				  ,[ShipToContact_TypeId]
				  ,[PO]
				  ,[Tax]
				  ,[EndUserEmailTo]
				  ,[Total]
				  ,[Subtotal]
				  ,[EmailTo]
				  ,[PO2]
				  ,[Notes]
                  ,[BCID]
				  ,[SystemModifiedOn])
				  VALUES
				  (
					 NEXT VALUE FOR [InstanceId]
					,{GescomType.Order}
					,{GescomConstant.BranchID}
					,{GescomType.Branch}
					,{GescomConstant.OrderTypeID}
					,{GescomType.OrderType}
					,NULL
					,{GescomType.SupportPerson}
					,{gescomCustomer.BillToAddress_InstanceId}
					,{GescomType.Address}
					,{gescomCustomer.BillToAddress_InstanceId}
					,{GescomType.Address}
					,NULL
					,{GescomType.Carrier}
					,{gescomCustomer.Account_InstanceId}
					,{GescomType.Account}
					,{GescomConstant.DeliveryTypeElectronic}
					,{GescomType.DeliveryType}
					,{GescomConstant.CurrencyID}
					,{GescomType.Currency}
					,{gescomCustomer.InstanceId}
					,{GescomType.Customer}
					,NULL
					,{GescomType.SalesPerson}
					,NULL
					,{GescomType.Contact}
					,NULL
					,{GescomType.Contact}
					,'*{bcCredit.PONumber}'
					,0
					,NULL
					,{-1 * Math.Abs(bcCredit.TotalAmountExcludingTax)}
					,{-1 * Math.Abs(bcCredit.TotalAmountExcludingTax)}
					,NULL
					,@PONumber
					,NULL
                    ,@CreditMemoBCID --USING CREDIT BCID FOR THE ORDER BCID
					,@PostingDate
				  )";

		await RunQueryWithParameterAsync<Model.BC.CreditMemo>(query, bcCredit);

        return (await GetOrderByUntrackedCreditAsync(bcCredit)).First().InstanceId;
    }


	public async Task<IEnumerable<Model.GescomEU.SalesOrder>> GetSalesOrderByOrderAsync(int orderId)
	{
		string query = $@"SELECT * FROM {_tableNames.Order} WHERE InstanceId = {orderId}";

		return await GetDbResultAsync<Model.GescomEU.SalesOrder>(query);
	}

	public async Task<IEnumerable<Model.GescomEU.SalesOrder>> GetSalesOrderByOrderAsync(Model.GescomEU.Order gescomOrder) => await GetSalesOrderByOrderAsync(gescomOrder.InstanceId);

	public async Task<IEnumerable<Model.GescomEU.SalesOrder>> GetSalesOrderByBCInvoice(Model.BC.Invoice bcInvoice)
	{
		string query = $"SELECT * FROM {_tableNames.SalesOrder} WHERE [Code] = '{bcInvoice.OrderNumber}'";

		return await GetDbResultAsync<Model.GescomEU.SalesOrder>(query);
	}

    public async Task<int> CreateSalesOrderReturnIdAsync(Model.BC.Invoice bcInvoice, int gescomOrderId)
    {
        string insertQuery = $@"INSERT INTO {_tableNames.SalesOrder}
								([InstanceId]
								  ,[TypeId]
								  ,[CreatedBy_InstanceId]
								  ,[CreatedBy_TypeId]
								  ,[DocumentType]
								  ,[Order_InstanceId]
								  ,[Order_TypeId]
								  ,[DocumentState]
								  ,[Notes]
								  ,[Code]
								  ,[CreatedOn]
								  ,[Status_InstanceId]
								  ,[Status_TypeId]
								  )
								  VALUES
								  (NEXT VALUE FOR [InstanceId]
								  ,{GescomType.SalesOrder}
								  ,{GescomConstant.UserID}
								  ,{GescomType.User}
								  ,1
								  ,{gescomOrderId}
								  ,{GescomType.Order}
								  ,{GescomConstant.DocumentStateProcessed}
								  ,NULL
								  ,@OrderNumber
								  ,@PostingDate
								  ,{GescomConstant.SalesOrderStatusProcessed}
								  ,916103220
								)";

		await RunQueryWithParameterAsync(insertQuery, bcInvoice);

		return (await GetSalesOrderByOrderAsync(gescomOrderId)).First().InstanceId;
    }

	public async Task CreateCreditSalesOrderAsync(Model.BC.CreditMemo bcCredit, int gescomOrderId)
	{
		string salesOrderCode = $"CAUT_S_{bcCredit.CreditMemoNumber}";

        string insertQuery = $@"INSERT INTO {_tableNames.SalesOrder}
								([InstanceId]
								  ,[TypeId]
								  ,[CreatedBy_InstanceId]
								  ,[CreatedBy_TypeId]
								  ,[DocumentType]
								  ,[Order_InstanceId]
								  ,[Order_TypeId]
								  ,[DocumentState]
								  ,[Notes]
								  ,[Code]
								  ,[CreatedOn]
								  ,[Status_InstanceId]
								  ,[Status_TypeId]
								  )
								  VALUES
								  (NEXT VALUE FOR [InstanceId]
								  ,{GescomType.SalesOrder}
								  ,{GescomConstant.UserID}
								  ,{GescomType.User}
								  ,1
								  ,{gescomOrderId}
								  ,{GescomType.Order}
								  ,{GescomConstant.DocumentStateProcessed}
								  ,NULL
								  ,'CAUT_S_{bcCredit.CreditMemoNumber}'
								  ,@PostingDate
								  ,{GescomConstant.SalesOrderStatusProcessed}
								  ,916103220
								)";

        await RunQueryWithParameterAsync(insertQuery, bcCredit);
    }



    public async Task<IEnumerable<Model.GescomEU.Invoice>> GetInvoiceByBCIDAsync(string bcid)
    {
        string query = $"SELECT * FROM {_tableNames.Invoice} WHERE [BCID] = '{bcid}'";

        return await GetDbResultAsync<Model.GescomEU.Invoice>(query);
    }

    public async Task<IEnumerable<Model.GescomEU.Invoice>> GetInvoiceByBCIDAsync(Model.BC.Invoice bcInvoice) => await GetInvoiceByBCIDAsync(bcInvoice.InvoiceBCID);

	public async Task<IEnumerable<Model.GescomEU.Invoice>> GetInvoiceByUntrackedCreditAsync(Model.BC.CreditMemo creditMemo) => await GetInvoiceByBCIDAsync(creditMemo.CreditMemoBCID);

    public async Task<int> CreateInvoiceReturnIdAsync(Model.BC.Invoice bcInvoice, int gescomOrderId)
    {
        string query = $@"INSERT INTO {_tableNames.Invoice}
								( [InstanceId]
								  ,[TypeId]
								  ,[CreatedBy_InstanceId]
								  ,[CreatedBy_TypeId]
								  ,[DocumentType]
								  ,[Order_InstanceId]
								  ,[Order_TypeId]
								  ,[DocumentState]
								  ,[Notes]
								  ,[Code]
								  ,[CreatedOn]
								  ,[Status_InstanceId]
								  ,[Status_TypeId]
								  ,[IsPaid]
								  ,[RemainingAmount]
								  ,[PaidOn]
								  ,[BCID]
								  )
								  VALUES
								  (NEXT VALUE FOR [InstanceId]
								  ,{GescomType.Invoice}
								  ,NULL
								  ,{GescomType.User}
								  ,{GescomConstant.DocumentTypeInvoice}
								  ,{gescomOrderId}
								  ,{GescomType.Order}
								  ,{GescomConstant.DocumentStateInvoicedCredited}
								  ,NULL
								  ,@InvoiceNumber
								  ,@PostingDate
								  ,{GescomConstant.InvoiceStatusInvoiced}
								  ,{GescomType.InvoiceStatus}
								  ,1
								  ,@TotalAmountExcludingTax
								  ,@PostingDate
								  ,@InvoiceBCID
								)";

        await RunQueryWithParameterAsync(query, bcInvoice);


        return (await GetInvoiceByBCIDAsync(bcInvoice)).First().InstanceId;
    }

	public async Task<int> CreateInvoiceForUntrackedCreditReturnIdAsync(Model.BC.CreditMemo bcCredit, int gescomOrderId)
	{
        string query = $@"INSERT INTO {_tableNames.Invoice}
								( [InstanceId]
								  ,[TypeId]
								  ,[CreatedBy_InstanceId]
								  ,[CreatedBy_TypeId]
								  ,[DocumentType]
								  ,[Order_InstanceId]
								  ,[Order_TypeId]
								  ,[DocumentState]
								  ,[Notes]
								  ,[Code]
								  ,[CreatedOn]
								  ,[Status_InstanceId]
								  ,[Status_TypeId]
								  ,[IsPaid]
								  ,[RemainingAmount]
								  ,[PaidOn]
								  ,[BCID]
								  )
								  VALUES
								  (NEXT VALUE FOR [InstanceId]
								  ,{GescomType.Invoice}
								  ,NULL
								  ,{GescomType.User}
								  ,{GescomConstant.DocumentTypeInvoice}
								  ,{gescomOrderId}
								  ,{GescomType.Order}
								  ,{GescomConstant.DocumentStateInvoicedCredited}
								  ,NULL
								  ,@CreditMemoNumber
								  ,@PostingDate
								  ,{GescomConstant.InvoiceStatusInvoiced}
								  ,{GescomType.InvoiceStatus}
								  ,1
								  ,@TotalAmountExcludingTax
								  ,NULL
								  ,@CreditMemoBCID
								)";

        await RunQueryWithParameterAsync(query, bcCredit);


        return (await GetInvoiceByUntrackedCreditAsync(bcCredit)).First().InstanceId;
    }



    public async Task CreateCreditDeliveryAsync(Model.BC.CreditMemo bcCredit, int gescomOrderId)
    {
        string query = $@"INSERT INTO {_tableNames.Delivery}
									([InstanceId]
									  ,[TypeId]
									  ,[CreatedBy_InstanceId]
									  ,[CreatedBy_TypeId]
									  ,[DocumentType]
									  ,[Order_InstanceId]
									  ,[Order_TypeId]
									  ,[DocumentState]
									  ,[Notes]
									  ,[Code]
									  ,[CreatedOn]
									  ,[Status_InstanceId]
									  ,[Status_TypeId]
									  ,[TrackingNumber]
									  )
									  VALUES
										  (NEXT VALUE FOR [InstanceId]
										  ,{GescomType.Delivery}
										  ,{GescomConstant.UserID}
										  ,{GescomType.User}
										  ,2
										  ,{gescomOrderId}
										  ,{GescomType.Order}
										  ,1
										  ,NULL
										  ,'CAUT_D_{bcCredit.CreditMemoNumber}'
										  ,NULL
										  ,{GescomConstant.DeliveryStatusProcessed}
										  ,{GescomType.DeliveryStatus}
										  ,NULL
										)";

        await RunQueryAsync(query);
    }

	public async Task CreateCreditAsync(Model.BC.CreditMemo bcCredit, int gescomOrderId)
	{
		string createCreditQuery = $@"INSERT INTO [GescomEU_merge].[dbo].[Invoice]
								( [InstanceId]
								  ,[TypeId]
								  ,[CreatedBy_InstanceId]
								  ,[CreatedBy_TypeId]
								  ,[DocumentType]
								  ,[Order_InstanceId]
								  ,[Order_TypeId]
								  ,[DocumentState]
								  ,[Notes]
								  ,[Code]
								  ,[CreatedOn]
								  ,[Status_InstanceId]
								  ,[Status_TypeId]
								  ,[IsPaid]
								  ,[RemainingAmount]
								  ,[PaidOn]
								  ,[BCID]
								  )
								  VALUES
								  (NEXT VALUE FOR [InstanceId]
								  ,{GescomType.Invoice}
								  ,NULL
								  ,{GescomType.User}
								  ,{GescomConstant.DocumentTypeInvoice}
								  ,{gescomOrderId}
								  ,{GescomType.Order}
								  ,{GescomConstant.DocumentStateInvoicedCredited}
								  ,NULL
								  ,@CreditMemoNumber
								  ,@PostingDate
								  ,1238
								  ,-273444347
								  ,1
								  ,{-1 * Math.Abs(bcCredit.TotalAmountExcludingTax)}
								  ,NULL
								  ,@CreditMemoBCID
								)";

		await RunQueryWithParameterAsync(createCreditQuery, bcCredit);

	}


	public async Task InsertOrderLinesAsync()
	{
//		string query = $@"INSERT INTO [GescomEU_merge].{_tableNames.OrderLine} (
//		[InstanceId]
//		,[TypeId]
//		,[Order_InstanceId]
//		,[Order_TypeId]
//		,[DebDesType]
//		,[Part_InstanceId]
//		,[Part_TypeId]
//		,[PartName]
//		,[Quantity]
//		,[UnitPrice]
//)

//SELECT next value for instanceid,
//{GescomType.OrderLine},
//o.InstanceId,
//{GescomType.Order},
//2,
//part.InstanceId,
//part.TypeId,
//prod.[Name],
//invl.quantity,
//invl.unitPrice
//from [GESCOM_BC_TEST].[dbo].BC_salesInvoiceLine invl
//join [GESCOM_BC_TEST].[dbo].BC_salesInvoices inv
//	on invl.documentId = inv.id
//join [GescomEU_merge].[ids].[Order_Tbl] o
//	on inv.orderId = o.bcid COLLATE database_default
//join [GescomEU_merge].dbo.[Product] prod
//	on invl.lineObjectNumber = prod.[Code] COLLATE database_default
//join [GescomEU_merge].dbo.[Part] part
//	on part.Product_InstanceId = prod.InstanceId";

		string query = $@"WITH CTE AS 
(
--INVOICE LINES
SELECT {GescomType.OrderLine} as OrderLineTypeId,
o.InstanceId as OrderInstanceId,
{GescomType.Order} OrderTypeId,
2 as DebDes,
part.InstanceId as PartInstanceId,
part.TypeId as PartTypeId,
prod.[Name],
1 as Quantity, --invl.quantity,
invl.netAmount
from [GESCOM_BC_TEST].[dbo].BC_salesInvoiceLine invl
join [GESCOM_BC_TEST].[dbo].BC_salesInvoices inv
	on invl.documentId = inv.id
join [GescomEU_merge].[ids].[Order_Tbl] o
	on inv.orderId = o.bcid COLLATE database_default
join [GescomEU_merge].dbo.[Product] prod
	on invl.lineObjectNumber = prod.[Code] COLLATE database_default
join [GescomEU_merge].dbo.[Part] part
	on part.Product_InstanceId = prod.InstanceId

UNION ALL

--CREDIT LINES
SELECT {GescomType.OrderLine} as OrderLineTypeId,
o.InstanceId as OrderInstanceId,
{GescomType.Order} OrderTypeId,
2 as DebDes,
part.InstanceId,
part.TypeId,
prod.[Name],
-1 as Quantity, --ABS(credline.quantity) * -1 as Quantity,
credline.netAmount
from [GESCOM_BC_TEST].[dbo].BC_salesCreditMemo cred
join [GESCOM_BC_TEST].[dbo].BC_salesCreditMemoLine credline
	on cred.id = credline.documentId
join GescomEU_merge.ids.Order_Tbl o
	on cred.id = o.BCID COLLATE database_default
join [GescomEU_merge].dbo.[Product] prod
	on credline.lineObjectNumber = prod.[Code] COLLATE database_default
join [GescomEU_merge].dbo.[Part] part
	on part.Product_InstanceId = prod.InstanceId
)


INSERT INTO [GescomEU_merge].[dbo].[OrderLine] (
									[InstanceId]
									,[TypeId]
									,[Order_InstanceId]
									,[Order_TypeId]
									,[DebDesType]
									,[Part_InstanceId]
									,[Part_TypeId]
									,[PartName]
									,[Quantity]
									,[UnitPrice]
   )
SELECT NEXT VALUE FOR [InstanceID]
,OrderLineTypeId
,OrderInstanceId
,OrderTypeId
,DebDes
,PartInstanceId
,PartTypeId
,[Name]
,Quantity
,NetAmount
FROM CTE";

		await RunQueryAsync(query);

	}


	public async Task ResolveDocumentLinesAsync()
	{
		//directly lifted this query from Rustam's merge scripts
		string query = @"--sales order document lines

declare @orderdocument_instanceId int;
declare @orderdocument_typeId int;
declare @notes varchar(max);
declare @branch_us_instanceId int;
declare @branch_us_typeId int;
declare @documentline_typeId int;

select @branch_us_instanceId = [instanceId], @branch_us_typeId = [TypeId]
From [GescomEU_merge].[dbo].[Branch]
WHERE [Name] = 'USA';
		
SET @documentline_typeId = [GescomEU_merge].[dbo].GetTypeId('DocumentLine');

SET @orderdocument_typeId = [GescomEU_merge].[dbo].GetTypeId('SalesOrder');
SET @orderdocument_instanceId = NULL;
SET @notes = NULL;

INSERT INTO [GescomEU_merge].[dbo].[DocumentLine] (
							   [InstanceId]
							  ,[TypeId]
							  ,[OrderLine_InstanceId]
							  ,[OrderLine_TypeId]
							  ,[OrderDocument_InstanceId]
							  ,[OrderDocument_TypeId]
							  ,[Note] )
							  SELECT NEXT VALUE FOR [InstanceId], @documentline_typeId, ol.InstanceId, ol.TypeId, od.InstanceId, od.TypeId, od.Notes
						FROM 
							[GescomEU_merge].[dbo].[Order] o 
							inner join [GescomEU_merge].[dbo].[OrderLine] ol ON ol.Order_InstanceId = o.InstanceId and ol.Order_TypeId = o.TypeId
							inner join [GescomEU_merge].[dbo].[OrderDocument] od ON od.Order_InstanceId = o.InstanceId and od.Order_TypeId = o.TypeId AND od.DocumentType = 1
						WHERE o.Branch_InstanceId = @branch_us_instanceId 
and o.bcid collate database_default in (select distinct orderId from GESCOM_BC_TEST.dbo.BC_salesInvoices
										union
										select distinct id from GESCOM_BC_TEST.dbo.BC_salesCreditMemo) 


--delivery document line
SET @orderdocument_typeId = [GescomEU_merge].[dbo].GetTypeId('Delivery');
SET @orderdocument_instanceId = NULL;
SET @notes = NULL;

INSERT INTO [GescomEU_merge].[dbo].[DocumentLine] (
			[InstanceId]
			,[TypeId]
			,[OrderLine_InstanceId]
			,[OrderLine_TypeId]
			,[OrderDocument_InstanceId]
			,[OrderDocument_TypeId]
			,[Note] )
			SELECT NEXT VALUE FOR [InstanceId], @documentline_typeId, ol.InstanceId, ol.TypeId, od.InstanceId, od.TypeId, od.Notes
	FROM 
		[GescomEU_merge].[dbo].[Order] o 
		inner join [GescomEU_merge].[dbo].[OrderLine] ol ON ol.Order_InstanceId = o.InstanceId and ol.Order_TypeId = o.TypeId
		inner join [GescomEU_merge].[dbo].[OrderDocument] od ON od.Order_InstanceId = o.InstanceId and od.Order_TypeId = o.TypeId AND od.DocumentType = 2
	WHERE o.Branch_InstanceId = @branch_us_instanceId 
	and o.bcid collate database_default in (select distinct orderId from GESCOM_BC_TEST.dbo.BC_salesInvoices
										union
										select distinct id from GESCOM_BC_TEST.dbo.BC_salesCreditMemo) 


--invoice document line
SET @orderdocument_typeId = [GescomEU_merge].[dbo].GetTypeId('Invoice');
SET @orderdocument_instanceId = NULL;
SET @notes = NULL;
INSERT INTO [GescomEU_merge].[dbo].[DocumentLine] (
			[InstanceId]
			,[TypeId]
			,[OrderLine_InstanceId]
			,[OrderLine_TypeId]
			,[OrderDocument_InstanceId]
			,[OrderDocument_TypeId]
			,[Note] )
			
	SELECT NEXT VALUE FOR [InstanceId], @documentline_typeId, ol.InstanceId, ol.TypeId, od.InstanceId, od.TypeId, od.Notes
	FROM 
		[GescomEU_merge].[dbo].[Order] o 
		inner join [GescomEU_merge].[dbo].[OrderLine] ol ON ol.Order_InstanceId = o.InstanceId and ol.Order_TypeId = o.TypeId
		inner join [GescomEU_merge].[dbo].[OrderDocument] od ON od.Order_InstanceId = o.InstanceId and od.Order_TypeId = o.TypeId AND od.DocumentType = 4
	WHERE o.Branch_InstanceId = @branch_us_instanceId 
	and o.bcid collate database_default in (select distinct orderId from GESCOM_BC_TEST.dbo.BC_salesInvoices
											union
											select distinct id from GESCOM_BC_TEST.dbo.BC_salesCreditMemo
											) ";

        await RunQueryAsync(query);
    }
}

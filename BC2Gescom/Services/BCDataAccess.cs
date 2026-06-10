using BC2Gescom.Configuration;
using Microsoft.Extensions.Options;

namespace BC2Gescom.Services;

public class BCDataAccess : DataAccess
{
    private readonly BCTableNames _tableNames;

    public BCDataAccess(IOptions<ConnectionStrings> appSettings, IOptions<BCTableNames> tableNames) : base(appSettings)
    {
        _connectionString = _appSettings.BCConnectionString;
        _tableNames = tableNames.Value;
    }

    public async Task<IEnumerable<Model.BC.LineItem>> GetInvoiceLinesAsync()
    {
        string query = $@"SELECT DISTINCT [id] as InvoiceLineID
      ,[documentId] as DocumentBCID
      ,[sequence]
      ,[itemId] as PartNumberID
      ,[accountId]
      ,[lineType]
      ,[lineObjectNumber] as PartNumber
      ,[description] as PartDescription
      ,[unitOfMeasureId]
      ,[unitOfMeasureCode]
      ,[unitPrice]
      ,[quantity]
      ,[discountAmount]
      ,[discountPercent]
      ,[discountAppliedBeforeTax]
      ,[amountExcludingTax]
      ,[taxCode]
      ,[taxPercent]
      ,[totalTaxAmount]
      ,[amountIncludingTax]
      ,[invoiceDiscountAllocation]
      ,[netAmount]
      ,[netTaxAmount]
      ,[netAmountIncludingTax]
      ,[shipmentDate]
      ,[itemVariantId]
  FROM {_tableNames.InvoiceLine}";

        return await GetDbResultAsync<Model.BC.LineItem>(query);
    }

    public async Task<IEnumerable<Model.BC.LineItem>> GetCreditLinesAsync()
    {
        string query = $@"SELECT DISTINCT [id] as InvoiceLineID
      ,[documentId] as DocumentBCID
      ,[sequence]
      ,[itemId] as PartNumberID
      ,[accountId]
      ,[lineType]
      ,[lineObjectNumber] as PartNumber
      ,[description] as PartDescription
      ,[unitOfMeasureId]
      ,[unitOfMeasureCode]
      ,[unitPrice]
      ,[quantity]
      ,[discountAmount]
      ,[discountPercent]
      ,[discountAppliedBeforeTax]
      ,[amountExcludingTax]
      ,[taxCode]
      ,[taxPercent]
      ,[totalTaxAmount]
      ,[amountIncludingTax]
      ,[invoiceDiscountAllocation]
      ,[netAmount]
      ,[netTaxAmount]
      ,[netAmountIncludingTax]
      ,[shipmentDate]
      ,[itemVariantId]
  FROM {_tableNames.CreditLine}";

        return await GetDbResultAsync<Model.BC.LineItem>(query);
    }

    public async Task<IEnumerable<Model.BC.Invoice>> GetInvoicesAsync()
    {
        string query = $@"SELECT DISTINCT [id] as InvoiceBCID
      ,[number] as InvoiceNumber
      ,[externalDocumentNumber] as PONumber
      ,[invoiceDate]
      ,[postingDate]
      ,[dueDate]
      ,[customerPurchaseOrderReference]
      ,[customerId] as CustomerBCId
      ,[customerNumber]
      ,[customerName]
      ,[billToName]
      ,[billToCustomerId]
      ,[billToCustomerNumber]
      ,[shipToName]
      ,[shipToContact]
      ,[sellToAddressLine1]
      ,[sellToAddressLine2]
      ,[sellToCity]
      ,[sellToCountry]
      ,[sellToState]
      ,[sellToPostCode]
      ,[billToAddressLine1]
      ,[billToAddressLine2]
      ,[billToCity]
      ,[billToCountry]
      ,[billToState]
      ,[billToPostCode]
      ,[shipToAddressLine1]
      ,[shipToAddressLine2]
      ,[shipToCity]
      ,[shipToCountry]
      ,[shipToState]
      ,[shipToPostCode]
      ,[currencyId]
      ,[currencyCode]
      ,[orderId]
      ,[orderNumber]
      ,[paymentTermsId]
      ,[shipmentMethodId]
      ,[salesperson]
      ,[pricesIncludeTax]
      ,[remainingAmount]
      ,[discountAmount]
      ,[discountAppliedBeforeTax]
      ,[totalAmountExcludingTax]
      ,[totalTaxAmount]
      ,[totalAmountIncludingTax]
      ,[status]
      ,[lastModifiedDateTime]
      ,[phoneNumber]
      ,[email]
  FROM {_tableNames.Invoice}";

        return await GetDbResultAsync<Model.BC.Invoice>(query);
    }

    public async Task<IEnumerable<Model.BC.CreditMemo>> GetCreditMemosAsync()
    {
        string query = $@"SELECT DISTINCT [id] as CreditMemoBCID
      ,[number] as CreditMemoNumber
      ,[externalDocumentNumber] as PONumber
      ,[creditMemoDate]
      ,[postingDate]
      ,[dueDate]
      ,[customerId] as CustomerBCID
      ,[customerNumber]
      ,[customerName]
      ,[billToName]
      ,[billToCustomerId]
      ,[billToCustomerNumber]
      ,[sellToAddressLine1]
      ,[sellToAddressLine2]
      ,[sellToCity]
      ,[sellToCountry]
      ,[sellToState]
      ,[sellToPostCode]
      ,[billToAddressLine1]
      ,[billToAddressLine2]
      ,[billToCity]
      ,[billToCountry]
      ,[billToState]
      ,[billToPostCode]
      ,[currencyId]
      ,[currencyCode]
      ,[paymentTermsId]
      ,[shipmentMethodId]
      ,[salesperson]
      ,[pricesIncludeTax]
      ,[discountAmount]
      ,[discountAppliedBeforeTax]
      ,[totalAmountExcludingTax]
      ,[totalTaxAmount]
      ,[totalAmountIncludingTax]
      ,[status]
      ,[lastModifiedDateTime]
      ,[invoiceId]
      ,[invoiceNumber]
      ,[phoneNumber]
      ,[email]
  FROM {_tableNames.Credit}";

        return await GetDbResultAsync<Model.BC.CreditMemo>(query);
    }

    public async Task<IEnumerable<Model.BC.Customer>> GetRelevantCustomersAsync()
    {
        string query = $@"SELECT DISTINCT cust.[id] as CustomerBCID
      ,cust.[number] as CustomerNumber
      ,[displayName] as CustomerName
      ,[addressLine1]
      ,[addressLine2]
      ,[city]
      ,[state]
      ,[country]
      ,[postalCode]
      ,[Customer_Posting_Group] as CustomerPostingGroup
      ,[Customer Type (Gescom)] as CustomerTypeGescom
      ,cust.[currencyCode]
      ,cust.[email]
      ,cust.[phoneNumber]
      ,[salespersonCode]
  FROM {_tableNames.Customer} cust
join {_tableNames.Invoice} inv
	on cust.id = inv.billToCustomerId
UNION
SELECT DISTINCT cust.[id] as CustomerBCID
      ,cust.[number] as CustomerNumber
      ,[displayName] as CustomerName
      ,[addressLine1]
      ,[addressLine2]
      ,[city]
      ,[state]
      ,[country]
      ,[postalCode]
      ,[Customer_Posting_Group] as CustomerPostingGroup
      ,[Customer Type (Gescom)] as CustomerTypeGescom
      ,cust.[currencyCode]
      ,cust.[email]
      ,cust.[phoneNumber]
      ,[salespersonCode]
from {_tableNames.Customer} cust
join {_tableNames.Credit} cred
	on cust.id = cred.billToCustomerId";

        return await GetDbResultAsync<Model.BC.Customer>(query);
    }

    public async Task<IEnumerable<Model.BC.CountryRegion>> GetCountryRegionsAsync()
    {
        string query = $"SELECT DISTINCT Id, [Code] as Abbreviation, displayName, [Tracking] FROM {_tableNames.CountryRegion} ORDER BY [Code]";

        return await GetDbResultAsync<Model.BC.CountryRegion>(query);
    }


    public async Task<IEnumerable<string>> GetDistinctPartNumbers()
    {
        string query = $@"WITH CTE AS 
(
select distinct lineobjectnumber
from {_tableNames.InvoiceLine}
union
select distinct lineobjectnumber
from {_tableNames.CreditLine}
)
SELECT DISTINCT lineObjectNumber FROM CTE ORDER BY lineObjectNumber";

        return await GetDbResultAsync<string>(query);
    }

    public async Task<IEnumerable<Model.Transient.Address>> GetRelevantCustomerAddressesAsync()
    {
        string query = $@"select distinct [addressLine1]
      ,[addressLine2]
      ,[city]
      ,[state]
      ,[postalCode]
	  ,[country] as CountryAbbr
from {_tableNames.Customer} cust
join {_tableNames.Invoice} inv
	on cust.id = inv.billToCustomerId
UNION
select distinct [addressLine1]
      ,[addressLine2]
      ,[city]
      ,[state]
      ,[postalCode]
	  ,[country] as CountryAbbr
from {_tableNames.Customer} cust
join {_tableNames.Credit} cred
	on cust.id = cred.billToCustomerId
  WHERE addressLine1 != ''
";

        return await GetDbResultAsync<Model.Transient.Address>(query);
    }

}

namespace BC2Gescom;

public static class GescomConstant
{
    public const int AccountingDepartmentID = 2866360;
    public const int BranchID = 3;
    public const int CurrencyID = 24;
    public const int CustomerTypeEndUser = 1217;
    public const int CustomerTypeVAR = 4935983;
    public const int DeliveryStatusProcessed = 1228;
    public const int DeliveryTypeElectronic = 1229;
    public const int DocumentTypeSalesOrder = 1;
    public const int DocumentTypeInvoice = 4;
    public const int DocumentTypeCredit = 6; 
    public const int LanguageID = 18;
    public const int InvoiceStatusInvoiced = 1238;
    public const int MarketID = 227;
    public const int OrderTypeID = 1255;
    public const int DocumentStateProcessed = 1;
    public const int DocumentStateInvoicedCredited = 3;
    public const int SalesOrderStatusProcessed = 1300;
    public const int UnknownAddressId = 4571175;
    public const int UserID = 2458627;

    public static int GetCustomerType(Model.BC.Customer customer)
    {
        return customer.CustomerTypeGescom == "End User" ? GescomConstant.CustomerTypeEndUser : GescomConstant.CustomerTypeVAR;
    }
}

namespace BC2Gescom;


public class BCApiInformation
{
    public string TokenEndpoint { get; set; } = string.Empty;
    public string ClientID { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public DataEndpoints DataEndpoints { get; set; }

    public BCApiInformation(string tokenEndpoint, string clientId, string clientSecret, DataEndpoints dataEndpoints)
    {
        TokenEndpoint = tokenEndpoint;
        ClientID = clientId;
        ClientSecret = clientSecret;
        DataEndpoints = dataEndpoints;
    }
}

public class DataEndpoints
{
    public string SalesInvoices                 { get; set; } = string.Empty;
    public string SalesInvoiceLines             { get; set; } = string.Empty;
    public string SalesCreditMemos              { get; set; } = string.Empty;
    public string SalesCreditMemoLines          { get; set; } = string.Empty;
    public string ItemAttributes                { get; set; } = string.Empty;
    public string Customers                     { get; set; } = string.Empty;
}

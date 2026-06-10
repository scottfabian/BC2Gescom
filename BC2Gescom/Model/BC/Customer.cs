using System.ComponentModel.DataAnnotations.Schema;

namespace BC2Gescom.Model.BC;

public class Customer
{
    public string CustomerBCID { get; set; } = string.Empty;
    public string CustomerNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CustomerPostingGroup { get; set; } = string.Empty;
    public string CustomerTypeGescom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public string StringifyAddress()
    {
        return  AddressLine1 + AddressLine2 + City + State + PostalCode + Country;
    }

}

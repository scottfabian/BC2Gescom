using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace BC2Gescom.Model.Transient;

public class Address
{
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryAbbr { get; set; } = string.Empty;

    public string Stringify()
    {
        return AddressLine1 + AddressLine2 + City + State + PostalCode + CountryAbbr;
    }
}

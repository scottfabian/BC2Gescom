namespace BC2Gescom.Model.GescomEU;

public class Address
{
    public int          InstanceId { get; set; }
    public int          TypeId { get; set; }
    public int?         Country_InstanceId { get; set; }
    public int?         Country_TypeId { get; set; }
    public string       StreetInfo { get; set; }                         = string.Empty;
    public string       AdditionalAddress { get; set; }                  = string.Empty;
    public string       Region { get; set; }                             = string.Empty;
    public string       ZipCode { get; set; }                            = string.Empty;
    public string       City { get; set; }                               = string.Empty;
    public string       StreetExtend { get; set; }                       = string.Empty;
    public string       Street { get; set; }                             = string.Empty;
    public DateTime?    SystemModifiedOn { get; set; }
    public DateTime     SystemCreatedOn { get; set; }
}

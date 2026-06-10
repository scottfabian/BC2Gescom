namespace BC2Gescom.Model.GescomEU;

public class Part
{
    public int          InstanceId { get; set; }
    public int          TypeId { get; set; }
    public int?         Product_InstanceId { get; set; }
    public int?         Product_TypeId { get; set; }
    public int?         Version_InstanceId { get; set; }
    public int?         Version_TypeId { get; set; }
    public string       PartCode { get; set; } = string.Empty;
    public bool?        IsObsoleted { get; set; }
    public DateTime?    SystemModifiedOn { get; set; }
    public DateTime     SystemCreatedOn { get; set; }
}

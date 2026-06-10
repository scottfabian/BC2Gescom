namespace BC2Gescom.Model.GescomEU;

public class DocumentLine
{
    public int          InstanceId { get; set; }
    public int          TypeId { get; set; }
    public int          OrderLine_InstanceId { get; set; }
    public int          OrderLine_TypeId { get; set; }
    public int?         OrderDocument_InstanceId { get; set; }
    public int?         OrderDocument_TypeId { get; set; }
    public string       Note { get; set; } = string.Empty;
    public DateTime?    SystemModifiedOn { get; set; }
    public DateTime     SystemCreatedOn { get; set; }
}

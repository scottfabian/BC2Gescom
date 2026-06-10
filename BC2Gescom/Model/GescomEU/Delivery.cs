namespace BC2Gescom.Model.GescomEU;

public class Delivery
{
    public int              InstanceId { get; set; }
    public int              TypeId { get; set; }
    public int?             CreatedBy_InstanceId { get; set; }
    public int?             CreatedBy_TypeId { get; set; }
    public int              DocumentType { get; set; }
    public int              Order_InstanceId { get; set; }
    public int              Order_TypeId { get; set; }
    public int              DocumentState { get; set; }
    public int?             OriginDocument_InstanceId { get; set; }
    public int?             OriginDocument_TypeId { get; set; }
    public string           Notes { get; set; } = string.Empty;
    public string           Code { get; set; } = string.Empty;
    public DateTime?        CreatedOn { get; set; }
    public int              Status_InstanceId { get; set; }
    public int              Status_TypeId { get; set; }
    public DateTime?        SendOn { get; set; }
    public string           TrackingNumber { get; set; } = string.Empty;
}

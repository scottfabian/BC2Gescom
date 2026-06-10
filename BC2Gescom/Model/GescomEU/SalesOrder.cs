namespace BC2Gescom.Model.GescomEU;

public class SalesOrder
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
    public string           Notes { get; set; }                               = string.Empty;
    public string           Code { get; set; }                                = string.Empty;
    public DateTime?        CreatedOn { get; set; }
    public int?             Status_InstanceId { get; set; }
    public int?             Status_TypeId { get; set; }
    public string           ExpectedDeliveryOn { get; set; }                  = string.Empty;
    public string           ShipmentDelay { get; set; }                       = string.Empty;
}

namespace BC2Gescom.Model.GescomEU;

public class OrderLine
{
    public int              InstanceId { get; set; }
    public int              TypeId { get; set; }
    public int?             Order_InstanceId { get; set; }
    public int?             Order_TypeId { get; set; }
    public int              DebDesType { get; set; }
    public int?             Part_InstanceId { get; set; }
    public int?             Part_TypeId { get; set; }
    public int?             OriginalLine_InstanceId { get; set; }
    public int?             OriginalLine_TypeId { get; set; }
    public int?             LineIndex { get; set; }
    public string           PartName { get; set; } = string.Empty;
    public DateTime?        EndOn { get; set; }
    public DateTime?        StartOn { get; set; }
    public int?             Quantity { get; set; }
    public decimal?         UnitPrice { get; set; }
    public string           Note { get; set; } = string.Empty;
    public DateTime?        SystemModifiedOn { get; set; }
    public DateTime         SystemCreatedOn { get; set; }
}

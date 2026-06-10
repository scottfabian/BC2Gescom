namespace BC2Gescom.Model.GescomEU;

public class Order
{
    public int          InstanceId { get; set; }
    public int          TypeId { get; set; }
    public int?         Branch_InstanceId { get; set; }
    public int?         Branch_TypeId { get; set; }
    public int          OrderType_InstanceId { get; set; }
    public int          OrderType_TypeId { get; set; }
    public int?         SupportPerson_InstanceId { get; set; }
    public int?         SupportPerson_TypeId { get; set; }
    public int?         ShipToAddress_InstanceId { get; set; }
    public int?         ShipToAddress_TypeId { get; set; }
    public int?         BillToAddress_InstanceId { get; set; }
    public int?         BillToAddress_TypeId { get; set; }
    public int?         Carrier_InstanceId { get; set; }
    public int?         Carrier_TypeId { get; set; }
    public int?         ShipTo_InstanceId { get; set; }
    public int?         ShipTo_TypeId { get; set; }
    public int?         DeliveryType_InstanceId { get; set; }
    public int?         DeliveryType_TypeId { get; set; }
    public int          Currency_InstanceId { get; set; }
    public int          Currency_TypeId { get; set; }
    public int          BillTo_InstanceId { get; set; }
    public int          BillTo_TypeId { get; set; }
    public int?         SalesPerson_InstanceId { get; set; }
    public int?         SalesPerson_TypeId { get; set; }
    public int?         BillToContact_InstanceId { get; set; }
    public int?         BillToContact_TypeId { get; set; }
    public int?         ShipToContact_InstanceId { get; set; }
    public int?         ShipToContact_TypeId { get; set; }
    public int?         NestedTo_InstanceId { get; set; }
    public int?         NestedTo_TypeId { get; set; }
    public string       ShippingNumber { get; set; }                 = string.Empty;
    public string       ShipmentDelay { get; set; }                  = string.Empty;
    public DateTime?    DeliveryOn { get; set; }                     
    public string       PO { get; set; }                             = string.Empty;
    public decimal?     Tax { get; set; }
    public string       EndUserEmailTo { get; set; }                 = string.Empty;
    public decimal?     Total { get; set; }
    public decimal?     Subtotal { get; set; }
    public string       EmailTo { get; set; }                        = string.Empty;
    public string       PO2 { get; set; }                            = string.Empty;
    public string       Notes { get; set; }                          = string.Empty;
    public DateTime?    SystemModifiedOn { get; set; }
    public DateTime     SystemCreatedOn { get; set; }
    public decimal?     Prepayment { get; set; }
    public string       InternalNote { get; set; }                   = string.Empty;
}
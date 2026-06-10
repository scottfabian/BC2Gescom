namespace BC2Gescom.Model.GescomEU;

public class Account
{
    public int          InstanceId { get; set; }
    public int          TypeId { get; set; }
    public int?         Branch_InstanceId { get; set; }
    public int?         Branch_TypeId { get; set; }
    public int?         Address_InstanceId { get; set; }
    public int?         Address_TypeId { get; set; }
    public int          Language_InstanceId { get; set; }
    public int          Language_TypeId { get; set; }
    public int          AccountType { get; set; }
    public int?         CreatedBy_InstanceId { get; set; }
    public int?         CreatedBy_TypeId { get; set; }
    public int?         DefaultCarrier_InstanceId { get; set; }
    public int?         DefaultCarrier_TypeId { get; set; }
    public int?         DefaultContact_InstanceId { get; set; }
    public int?         DefaultContact_TypeId { get; set; }
    public bool?        IsShipmentPaid { get; set; }
    public string       SmacCode { get; set; }                                        = string.Empty;
    public bool?        IsByRoad { get; set; }                                        
    public string       ShippingAct { get; set; }                                     = string.Empty;
    public bool?        IsObsoleted { get; set; }                                    
    public string       ProductionNotes { get; set; }                                 = string.Empty;
    public string       Code { get; set; }                                            = string.Empty;
    public DateTime?    CreatedOn { get; set; }                                
    public string       Message { get; set; }                                         = string.Empty;
    public string       EmailTo { get; set; }                                         = string.Empty;
    public string       Web { get; set; }                                             = string.Empty;
    public string       OrderingNotes { get; set; }                                   = string.Empty;
    public string       Phone { get; set; }                                           = string.Empty;
    public string       Name { get; set; }                                            = string.Empty;
    public DateTime?    SystemModifiedOn { get; set; }                              
    public DateTime     SystemCreatedOn { get; set; }                               
    public string       SupportProductCode { get; set; }                              = string.Empty;
    public string       SupportClientCode { get; set; }                               = string.Empty;
    public int?         Industry_InstanceId { get; set; }
    public int?         Industry_TypeId { get; set; }
    public bool?        IsTCP { get; set; }
    public bool?        IsTCI { get; set; }
    public string?      BCID { get; set; }
}

namespace BC2Gescom.Model.GescomEU;

public class Contact
{
    public int              InstanceId { get; set; }
    public int              TypeId { get; set; }
    public int?             Prefix_InstanceId { get; set; }
    public int?             Prefix_TypeId { get; set; }
    public int              Gender { get; set; }
    public int?             Account_InstanceId { get; set; }
    public int?             Account_TypeId { get; set; }
    public string           Position { get; set; }                        = string.Empty;
    public string           Responsibility { get; set; }                  = string.Empty;
    public DateTime?        CreatedOn { get; set; }                       
    public bool?            IsObsolete { get; set; }                      
    public string           FamilyName { get; set; }                      = string.Empty;
    public string           Fax { get; set; }                             = string.Empty;
    public string           Mobile { get; set; }                          = string.Empty;
    public string           Comments { get; set; }                        = string.Empty;
    public string           Phone { get; set; }                           = string.Empty;
    public string           EMail { get; set; }                           = string.Empty;
    public string           FirstName { get; set; }                       = string.Empty;
    public DateTime?        SystemModifiedOn { get; set; }
    public DateTime         SystemCreatedOn { get; set; }
    public bool?            ReceivePrices { get; set; }
    public bool?            ReceiveNews { get; set; }
}

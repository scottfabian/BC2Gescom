namespace BC2Gescom.Model.GescomEU;

public class Country
{
    public int              InstanceId { get; set; }
    public int              TypeId { get; set; }
    public int?             Branch_InstanceId { get; set; }
    public int?             Branch_TypeId { get; set; }
    public int?             Market_InstanceId { get; set; }
    public int?             Market_TypeId { get; set; }
    public int?             Currency_InstanceId { get; set; }
    public int?             Currency_TypeId { get; set; }
    public decimal?         DEBFactor { get; set; }
    public string           DefaultLanguage { get; set; }                         = string.Empty;
    public string           Code { get; set; }                                    = string.Empty;
    public string           Name { get; set; }                                    = string.Empty;
    public string           Alpha3 { get; set; }                                  = string.Empty;
    public string           Alpha2 { get; set; }                                  = string.Empty;
    public DateTime?        SystemModifiedOn { get; set; }                        
    public DateTime         SystemCreatedOn { get; set; }
}

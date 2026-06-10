namespace BC2Gescom.Model.GescomEU;

public class Product
{
    public int              InstanceId { get; set; }
    public int              TypeId { get; set; }
    public int?             Branch_InstanceId { get; set; }
    public int?             Branch_TypeId { get; set; }
    public int?             OEMGroup_InstanceId { get; set; }
    public int?             OEMGroup_TypeId { get; set; }
    public int?             ProductClass_InstanceId { get; set; }
    public int?             ProductClass_TypeId { get; set; }
    public int?             LicenseType_InstanceId { get; set; }
    public int?             LicenseType_TypeId { get; set; }
    public int?             CustomsCode_InstanceId { get; set; }
    public int?             CustomsCode_TypeId { get; set; }
    public int?             AccountingCategory_InstanceId { get; set; }
    public int?             AccountingCategory_TypeId { get; set; }
    public int?             Family_InstanceId { get; set; }
    public int?             Family_TypeId { get; set; }
    public int?             DevGroup_InstanceId { get; set; }
    public int?             DevGroup_TypeId { get; set; }
    public int?             Edition_InstanceId { get; set; }
    public int?             Edition_TypeId { get; set; }
    public int?             TargetMarket_InstanceId { get; set; }
    public int?             TargetMarket_TypeId { get; set; }
    public int?             ProductCategory_InstanceId { get; set; }
    public int?             ProductCategory_TypeId { get; set; }
    public int?             Currency_InstanceId { get; set; }
    public int?             Currency_TypeId { get; set; }
    public bool?            IsRenewal { get; set; }
    public bool?            IsFixedPrice { get; set; }
    public decimal?         Weight { get; set; }
    public decimal?         ReInvoiceFactor { get; set; }
    public bool?            IsAddon { get; set; }
    public bool?            IsHided { get; set; }
    public short?           FreeDuration { get; set; }
    public bool?            IsObsoleted { get; set; }
    public decimal?         Price { get; set; }                                    
    public string           Note { get; set; }                                       = string.Empty;
    public string           Code { get; set; }                                       = string.Empty;
    public string           Name { get; set; }                                       = string.Empty;
    public short?           Duration { get; set; }                                   
    public bool?            IsVM { get; set; }                                        
    public string           Description { get; set; }                                = string.Empty;
    public DateTime?        SystemModifiedOn { get; set; }
    public DateTime         SystemCreatedOn { get; set; }
    public int              DiscountCategory { get; set; }
    public decimal?         UniversalResellerDiscount { get; set; }
}

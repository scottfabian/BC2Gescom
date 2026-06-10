namespace BC2Gescom.Model.GescomEU;

public class Customer
{
    public int              InstanceId { get; set; }
    public int              TypeId { get; set; }
    public int?             SalesPerson_InstanceId { get; set; }
    public int?             SalesPerson_TypeId { get; set; }
    public int              AccountingDepartment_InstanceId { get; set; }
    public int              AccountingDepartment_TypeId { get; set; }
    public int?             TermMode_InstanceId { get; set; }
    public int?             TermMode_TypeId { get; set; }
    public int?             CustomerType_InstanceId { get; set; }
    public int?             CustomerType_TypeId { get; set; }
    public int?             Currency_InstanceId { get; set; }
    public int?             Currency_TypeId { get; set; }
    public int?             SageExportSession_InstanceId { get; set; }
    public int?             SageExportSession_TypeId { get; set; }
    public int?             BlockedBy_InstanceId { get; set; }
    public int?             BlockedBy_TypeId { get; set; }
    public int?             CreatedBy_InstanceId { get; set; }
    public int?             CreatedBy_TypeId { get; set; }
    public int?             Term_InstanceId { get; set; }
    public int?             Term_TypeId { get; set; }
    public int?             PaymentMethod_InstanceId { get; set; }
    public int?             PaymentMethod_TypeId { get; set; }
    public int?             InvoiceMethod_InstanceId { get; set; }
    public int?             InvoiceMethod_TypeId { get; set; }
    public int              Account_InstanceId { get; set; }
    public int              Account_TypeId { get; set; }
    public int?             BillToAddress_InstanceId { get; set; }
    public int?             BillToAddress_TypeId { get; set; }
    public string           AuxiliaryAccountEx { get; set; }                       = string.Empty;
    public string           AuxiliaryAccount { get; set; }                         = string.Empty;
    public decimal?         SubNoSupportFactor { get; set; }                      
    public decimal?         SMAFactor { get; set; }                               
    public decimal?         PerpetualSMARenewalFactor { get; set; }               
    public decimal?         PerpetualNoSupportFactor { get; set; }                
    public int?             TermDay { get; set; }                                 
    public int?             MonthDay { get; set; }                                
    public int?             TermTolerance { get; set; }                           
    public int?             TermPeriod { get; set; }                              
    public string           PaymentRecoveryEmailTo { get; set; }                   = string.Empty;
    public string           CodeTVA { get; set; }                                  = string.Empty;
    public string           AccountingCode { get; set; }                           = string.Empty;
    public string           AccountingName { get; set; }                           = string.Empty;
    public DateTime?        CreatedOn { get; set; }                               
    public string           SalesOrderEmailTo { get; set; }                        = string.Empty;
    public string           RenewalEmailTo { get; set; }                           = string.Empty;
    public string           InvoiceEmailTo { get; set; }                           = string.Empty;
    public decimal?         CreditLimit { get; set; }                             
    public DateTime?        BlockedOn { get; set; }                               
    public decimal?         Balance { get; set; }                                 
    public string           AccountingNotes { get; set; }                          = string.Empty;
    public DateTime?        SystemModifiedOn { get; set; }                       
    public DateTime         SystemCreatedOn { get; set; }                        
    public decimal?         PerpetualSupportFactor { get; set; }                 
    public decimal?         PerpetualSMAGoldRenewalFactor { get; set; }          
    public decimal?         SubSupportFactor { get; set; }                       
    public decimal?         SubNoSupportRenewalFactor { get; set; }              
    public decimal?         SubSupportRenewalFactor { get; set; }                
    public string           DeliveryOrderEMail { get; set; }                       = string.Empty;
    public string?          BCID { get; set; }
}

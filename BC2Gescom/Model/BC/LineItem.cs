using System.ComponentModel.DataAnnotations.Schema;

namespace BC2Gescom.Model.BC;

public class LineItem
{
    public string           InvoiceLineID { get; set; }                                      = string.Empty;                                                        
    public string           DocumentBCID { get; set; }                                       = string.Empty;                                                                                  
    public int              Sequence { get; set; }                                                             
    public string           PartNumberID { get; set; }                                       = string.Empty;                                                                              
    public string           AccountId { get; set; }                                          = string.Empty;
    public string           LineType { get; set; }                                           = string.Empty;  
    public string           PartNumber { get; set; }                                         = string.Empty;                                                   
    public string           PartDescription { get; set; }                                    = string.Empty;                                                                              
    public string           UnitOfMeasureId { get; set; }                                    = string.Empty;
    public string           UnitOfMeasureCode { get; set; }                                  = string.Empty;
    public decimal          UnitPrice { get; set; }                                         
    public decimal          Quantity { get; set; }                                          
    public decimal          DiscountAmount { get; set; }                                    
    public decimal          DiscountPercent { get; set; }                                   
    public string           DiscountAppliedBeforeTax { get; set; }                           = string.Empty;
    public decimal          AmountExcludingTax { get; set; }                                
    public string           TaxCode { get; set; }                                            = string.Empty;
    public decimal          TaxPercent { get; set; }                                        
    public decimal          TotalTaxAmount { get; set; }                                    
    public decimal          AmountIncludingTax { get; set; }                                
    public decimal          InvoiceDiscountAllocation { get; set; }                         
    public decimal          NetAmount { get; set; }                                         
    public decimal          NetTaxAmount { get; set; }                                      
    public decimal          NetAmountIncludingTax { get; set; }                             
    public DateTime         ShipmentDate { get; set; }                                     
    public string           ItemVariantId { get; set; }                                      = string.Empty;
}

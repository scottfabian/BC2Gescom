using System.ComponentModel.DataAnnotations.Schema;

namespace BC2Gescom.Model.BC;

public class Invoice
{
    public string       InvoiceBCID { get; set; }                           = string.Empty;                                                     
    public string       InvoiceNumber { get; set; }                         = string.Empty;
    public string       PONumber { get; set; }                              = string.Empty;                                                                      
    public string       InvoiceDate { get; set; }                           = string.Empty;
    public string       PostingDate { get; set; }                           = string.Empty;
    public string       DueDate { get; set; }                               = string.Empty;
    public string       CustomerPurchaseOrderReference { get; set; }        = string.Empty;
    public string       CustomerBCID { get; set; }                          = string.Empty;
    public string       CustomerNumber { get; set; }                        = string.Empty;
    public string       CustomerName { get; set; }                          = string.Empty;
    public string       BillToName { get; set; }                            = string.Empty;
    public string       BillToCustomerId { get; set; }                      = string.Empty;
    public string       BillToCustomerNumber { get; set; }                  = string.Empty;
    public string       ShipToName { get; set; }                            = string.Empty;
    public string       ShipToContact { get; set; }                         = string.Empty;
    public string       SellToAddressLine1 { get; set; }                    = string.Empty;
    public string       SellToAddressLine2 { get; set; }                    = string.Empty;
    public string       SellToCity { get; set; }                            = string.Empty;
    public string       SellToCountry { get; set; }                         = string.Empty;
    public string       SellToState { get; set; }                           = string.Empty;
    public string       SellToPostCode { get; set; }                        = string.Empty;
    public string       BillToAddressLine1 { get; set; }                    = string.Empty;
    public string       BillToAddressLine2 { get; set; }                    = string.Empty;
    public string       BillToCity { get; set; }                            = string.Empty;
    public string       BillToCountry { get; set; }                         = string.Empty;
    public string       BillToState { get; set; }                           = string.Empty;
    public string       BillToPostCode { get; set; }                        = string.Empty;
    public string       ShipToAddressLine1 { get; set; }                    = string.Empty;
    public string       ShipToAddressLine2 { get; set; }                    = string.Empty;
    public string       ShipToCity { get; set; }                            = string.Empty;
    public string       ShipToCountry { get; set; }                         = string.Empty;
    public string       ShipToState { get; set; }                           = string.Empty;
    public string       ShipToPostCode { get; set; }                        = string.Empty;
    public string       CurrencyId { get; set; }                            = string.Empty;
    public string       CurrencyCode { get; set; }                          = string.Empty;
    public string       OrderId { get; set; }                               = string.Empty;
    public string       OrderNumber { get; set; }                           = string.Empty;
    public string       PaymentTermsId { get; set; }                        = string.Empty;
    public string       ShipmentMethodId { get; set; }                      = string.Empty;
    public string       Salesperson { get; set; }                           = string.Empty;
    public string       PricesIncludeTax { get; set; }                      = string.Empty;
    public decimal      RemainingAmount { get; set; }
    public decimal      DiscountAmount { get; set; }
    public string       DiscountAppliedBeforeTax { get; set; }              = string.Empty;
    public decimal      TotalAmountExcludingTax { get; set; }               
    public decimal      TotalTaxAmount { get; set; }                        
    public decimal      TotalAmountIncludingTax { get; set; }               
    public string       Status { get; set; }                                = string.Empty;
    public string       LastModifiedDateTime { get; set; }                  = string.Empty;
    public string       PhoneNumber { get; set; }                           = string.Empty;
    public string       Email { get; set; }                                 = string.Empty;
}

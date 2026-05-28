using Domain.Common;

namespace Domain.Entities;

public class InvoiceDetail : BaseEntity
{
    public int InvoiceId { get; set; }
    public string Concept { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }

    //Navegacion
    public Invoice? Invoice { get; set; }
}
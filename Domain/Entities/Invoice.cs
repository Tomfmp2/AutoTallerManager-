using Domain.Common;

namespace Domain.Entities;

public class Invoice : BaseEntity
{
    public int WorkshopId { get; set; }
    public int ServiceOrderId { get; set; }
    public int PaymentMethodId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public string PaymentStatus { get; set; } = "Pendiente";
    public decimal LaborCost { get; set; } = 0;
    public decimal Subtotal { get; set; }
    public decimal Taxes { get; set; } = 0;
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }
    // Navegacion
    public Workshop? Workshop { get; set; }
    public ServiceOrder? ServiceOrder { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
}

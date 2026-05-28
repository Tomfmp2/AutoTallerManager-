using Domain.Common;

namespace Domain.Entities;

public class ServiceOrderPart : BaseEntity
{
    public int ServiceOrderId { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal AppliedUnitPrice { get; set; }

    // Navegacion

    public ServiceOrder? ServiceOrder { get; set; }
    public Part? Part { get; set; }
}

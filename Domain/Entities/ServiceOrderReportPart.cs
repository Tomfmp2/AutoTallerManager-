using Domain.Common;

namespace Domain.Entities;

public class ServiceOrderReportPart : BaseEntity
{
    public int ServiceOrderReportId { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceSnapshot { get; set; }

    // Navegación
    public ServiceOrderReport? ServiceOrderReport { get; set; }
    public Part? Part { get; set; }
}

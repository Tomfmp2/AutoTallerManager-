using Domain.Common;

namespace Domain.Entities;

public class ServiceOrderReport : BaseEntity
{
    public int ServiceOrderId { get; set; }
    public int MechanicId { get; set; }
    public string ReportText { get; set; } = string.Empty;
    public bool IsDiagnostic { get; set; }
    public int? EstimatedHours { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public ServiceOrder? ServiceOrder { get; set; }
    public User? Mechanic { get; set; }
    public ICollection<ServiceOrderReportPart> ReportParts { get; set; } = new List<ServiceOrderReportPart>();
}

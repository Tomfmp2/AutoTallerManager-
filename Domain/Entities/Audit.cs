using Domain.Common;

namespace Domain.Entities;

public class Audit : BaseEntity
{
    public int WorkShopId { get; set; }
    public int UserId { get; set; }
    public int AuditActionTypeId { get; set; }
    public string AffectedEntity { get; set; } = string.Empty;
    public int AffectedRecordId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? PreviousValues { get; set; }
    public string? NewValues { get; set; }

    // Navegacion

    public Workshop? Workshop { get; set; }
    public User? User { get; set; }
    public AuditActionType? AuditActionType { get; set; }
}
using Domain.Common;

namespace Domain.Entities;

public class VehiclePhoto : BaseEntity
{
    public int VehicleId { get; set; }
    public string PhotoData { get; set; } = string.Empty; // base64 data URL
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegacion
    public Vehicle? Vehicle { get; set; }
}

using Domain.Common;

namespace Domain.Entities;

public class Vehicle : BaseEntity
{
    public int WorkShopId { get; set; }
    public int ModeId { get; set; }
    public int ColorId { get; set; }
    public string? LicensePlate { get; set; }
    public string VIN { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Mileage { get; set; } = 0;
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }

    // Navegacion
    public Workshop? Workshop { get; set; }
    public VehicleModel? Model { get; set; }
    public VehicleColor? Color { get; set; }
    public ICollection<VehicleOwnerHistory> OwnerHistories { get; set; } = new List<VehicleOwnerHistory>();
    public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
}

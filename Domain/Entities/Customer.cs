using Domain.Common;

namespace Domain.Entities;

public class Customer : BaseEntity
{
    public int WorkShopId { get; set; }
    public int PersonId { get; set; }
    public string? AddressStreet { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressState { get; set; }
    public string? AddressZipCode { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }

    // Navegacion
    public Workshop? Workshop { get; set; }
    public Person? Person { get; set; }
    public ICollection<VehicleOwnerHistory> VehicleOwnerHistories { get; set; } = new List<VehicleOwnerHistory>();
}

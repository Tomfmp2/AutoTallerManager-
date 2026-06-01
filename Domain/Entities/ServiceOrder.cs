using Domain.Common;

namespace Domain.Entities;

public class ServiceOrder : BaseEntity
{
    public int WorkshopId { get; set; }
    public int VehicleId { get; set; }
    public int ServiceTypeId { get; set; }
    public int? MechanicId { get; set; }
    public int? ReceptionistId { get; set; }
    public int OrderStatusId { get; set; }
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string? WorkPerformed { get; set; }
    public string? Observations { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }
    
    // Navegacion
    public Workshop? Workshop { get; set; }
    public Vehicle? Vehicle { get; set; }
    public ServiceType? ServiceType { get; set; }
    public User? Mechanic { get; set; }
    public User? Receptionist { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public ICollection<ServiceOrderPart> ServiceOrderParts { get; set; } = new List<ServiceOrderPart>();
    public Invoice? Invoice { get; set; }
}
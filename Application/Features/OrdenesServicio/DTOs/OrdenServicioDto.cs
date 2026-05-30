namespace Application.Features.OrdenesServicio.DTOs;

public class OrdenServicioDto
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = null!;
    public int VehicleId { get; set; }
    public string VehiclePlaca { get; set; } = null!;
    public string ClienteNombre { get; set; } = null!;
    public int ServiceTypeId { get; set; }
    public string TipoServicio { get; set; } = null!;
    public int AssignedMechanicId { get; set; }
    public string MecanicoNombre { get; set; } = null!;
    public int Estado { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public decimal MontoEstimado { get; set; }
    public string? Descripcion { get; set; }
    public DateTime CreatedAt { get; set; }
}
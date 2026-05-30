namespace Application.Features.OrdenesServicio.DTOs;

public class UpdateOrdenServicioDto
{
    public int ServiceTypeId { get; set; }
    public int AssignedMechanicId { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public string? Descripcion { get; set; }
}
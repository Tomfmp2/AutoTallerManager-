namespace Application.Features.Facturas.DTOs;

public class GenerarFacturaDto
{
    public Guid OrdenServicioId { get; set; }
    public decimal ManodeObra { get; set; }
}
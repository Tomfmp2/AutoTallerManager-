namespace Application.Features.Facturas.DTOs;

public class FacturaDto
{
    public int Id { get; set; }
    public string NumeroFactura { get; set; } = null!;
    public int OrdenServicioId { get; set; }
    public string NumeroOrden { get; set; } = null!;
    public string ClienteNombre { get; set; } = null!;
    public string Vehiculo { get; set; } = null!;
    public DateTime FechaFactura { get; set; }
    public List<DetalleOrdenDto> Detalles { get; set; } = new();
    public decimal MontoRepuestos { get; set; }
    public decimal ManodeObra { get; set; }
    public decimal Subtotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
}
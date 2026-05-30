namespace Application.Features.Facturas.DTOs;

public class DetalleOrdenDto
{
    public Guid RepuestoId { get; set; }
    public string RepuestoCodigo { get; set; } = null!;
    public string RepuestoDescripcion { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
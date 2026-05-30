namespace Application.Features.Repuestos.DTOs;

public class CreateRepuestoDto
{
    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal PrecioUnitario { get; set; }
    public int PartCategoryId { get; set; }
    public int StockActual { get; set; }
}
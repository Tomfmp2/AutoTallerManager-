namespace Application.Features.Repuestos.DTOs;

public class RepuestoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal PrecioUnitario { get; set; }
    public int PartCategoryId { get; set; }
    public string Categoria { get; set; } = null!;
    public int StockActual { get; set; }
    public bool StockDisponible { get; set; }
    public int TotalUtilizaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}
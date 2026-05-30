using Application.Common;
using MediatR;

namespace Application.Features.Repuestos.Commands.CreateRepuesto;

public class CreateRepuestoCommand : IRequest<Result<int>>
{
    public int WorkshopId     { get; set; }
    public int PartCategoryId { get; set; }
    public string Codigo      { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal PrecioUnitario { get; set; }
    public int StockInicial   { get; set; }
    public int StockMinimo    { get; set; }
    public string? Ubicacion  { get; set; }
    public string? Marca      { get; set; }
}

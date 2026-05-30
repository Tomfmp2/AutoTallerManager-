using Application.Common;
using MediatR;

namespace Application.Features.Repuestos.Commands.UpdateRepuesto;

public class UpdateRepuestoCommand : IRequest<Result>
{
    public int Id             { get; set; }
    public string Descripcion { get; set; } = null!;
    public decimal PrecioUnitario { get; set; }
    public int StockMinimo    { get; set; }
    public string? Ubicacion  { get; set; }
    public string? Marca      { get; set; }
}

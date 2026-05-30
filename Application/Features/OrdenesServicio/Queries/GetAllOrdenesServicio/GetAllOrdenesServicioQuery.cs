using Application.Common;
using Application.Features.OrdenesServicio.DTOs;
using MediatR;

namespace Application.Features.OrdenesServicio.Queries.GetAllOrdenesServicio;

public class GetAllOrdenesServicioQuery : IRequest<Result<PagedResult<OrdenServicioDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize   { get; set; } = 10;
    public int? EstadoId  { get; set; }
}

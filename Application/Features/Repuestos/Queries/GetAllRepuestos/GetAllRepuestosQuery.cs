using Application.Common;
using Application.Features.Repuestos.DTOs;
using MediatR;

namespace Application.Features.Repuestos.Queries.GetAllRepuestos;

public class GetAllRepuestosQuery : IRequest<Result<PagedResult<RepuestoDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize   { get; set; } = 10;
    public string? Search { get; set; }
}

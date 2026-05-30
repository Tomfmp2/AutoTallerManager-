using Application.Common;
using Application.Features.Clientes.DTOs;
using MediatR;

namespace Application.Features.Clientes.Queries.GetAllClientes;

public class GetAllClientesQuery : IRequest<Result<PagedResult<ClienteDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
}

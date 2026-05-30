using Application.Common;
using Application.Features.Clientes.DTOs;
using MediatR;

namespace Application.Features.Clientes.Queries.GetClienteById;

public class GetClienteByIdQuery : IRequest<Result<ClienteDto>>
{
    public int Id { get; set; }
}

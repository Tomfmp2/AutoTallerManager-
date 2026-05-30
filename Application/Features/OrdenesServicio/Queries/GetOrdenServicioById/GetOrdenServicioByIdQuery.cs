using Application.Common;
using Application.Features.OrdenesServicio.DTOs;
using MediatR;

namespace Application.Features.OrdenesServicio.Queries.GetOrdenServicioById;

public class GetOrdenServicioByIdQuery : IRequest<Result<OrdenServicioDto>>
{
    public int Id { get; set; }
}

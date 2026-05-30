using Application.Common;
using Application.Features.Repuestos.DTOs;
using MediatR;

namespace Application.Features.Repuestos.Queries.GetRepuestoById;

public class GetRepuestoByIdQuery : IRequest<Result<RepuestoDto>>
{
    public int Id { get; set; }
}

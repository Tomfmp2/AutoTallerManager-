using Application.Common;
using Application.Features.Vehiculos.DTOs;
using MediatR;

namespace Application.Features.Vehiculos.Queries.GetVehiculoById;

public class GetVehiculoByIdQuery : IRequest<Result<VehiculoDto>>
{
    public int Id { get; set; }
}

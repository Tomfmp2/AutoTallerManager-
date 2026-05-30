using Application.Common;
using MediatR;

namespace Application.Features.Vehiculos.Commands.DeleteVehiculo;

public class DeleteVehiculoCommand : IRequest<Result>
{
    public int Id { get; set; }
}

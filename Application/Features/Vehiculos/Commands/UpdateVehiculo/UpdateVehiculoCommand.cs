using Application.Common;
using MediatR;

namespace Application.Features.Vehiculos.Commands.UpdateVehiculo;

public class UpdateVehiculoCommand : IRequest<Result>
{
    public int Id { get; set; }
    public string? Placa { get; set; }
    public int Kilometraje { get; set; }
    public string? Notas { get; set; }
}

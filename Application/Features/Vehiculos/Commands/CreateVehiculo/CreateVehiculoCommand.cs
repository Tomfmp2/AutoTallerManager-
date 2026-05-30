using Application.Common;
using MediatR;

namespace Application.Features.Vehiculos.Commands.CreateVehiculo;

public class CreateVehiculoCommand : IRequest<Result<int>>
{
    public int WorkshopId { get; set; }
    public int CustomerId { get; set; }
    public int ModelId { get; set; }
    public int ColorId { get; set; }
    public string? Placa { get; set; }
    public string VIN { get; set; } = null!;
    public int Anio { get; set; }
    public int Kilometraje { get; set; }
    public string? Notas { get; set; }
}

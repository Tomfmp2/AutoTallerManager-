using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehiculos.Commands.UpdateVehiculo;

public class UpdateVehiculoCommandHandler : IRequestHandler<UpdateVehiculoCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateVehiculoCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateVehiculoCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _db.Vehicles
            .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive, cancellationToken);

        if (vehicle is null)
            return Result.Failure($"No se encontró el vehículo con ID {request.Id}.");

        vehicle.LicensePlate = request.Placa;
        vehicle.Mileage      = request.Kilometraje;
        vehicle.Notes        = request.Notas;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

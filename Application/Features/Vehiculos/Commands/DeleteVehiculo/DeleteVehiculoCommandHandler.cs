using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehiculos.Commands.DeleteVehiculo;

public class DeleteVehiculoCommandHandler : IRequestHandler<DeleteVehiculoCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteVehiculoCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeleteVehiculoCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _db.Vehicles
            .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive, cancellationToken);

        if (vehicle is null)
            return Result.Failure($"No se encontró el vehículo con ID {request.Id}.");

        vehicle.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

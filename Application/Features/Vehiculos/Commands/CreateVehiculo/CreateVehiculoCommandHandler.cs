using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Vehiculos.Commands.CreateVehiculo;

public class CreateVehiculoCommandHandler : IRequestHandler<CreateVehiculoCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _db;

    public CreateVehiculoCommandHandler(IUnitOfWork unitOfWork, IApplicationDbContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public async Task<Result<int>> Handle(CreateVehiculoCommand request, CancellationToken cancellationToken)
    {
        var vehicle = new Vehicle
        {
            WorkshopId  = request.WorkshopId,
            ModelId     = request.ModelId,
            ColorId     = request.ColorId,
            LicensePlate = request.Placa,
            VIN         = request.VIN,
            Year        = request.Anio,
            Mileage     = request.Kilometraje,
            Notes       = request.Notas,
            IsActive    = true
        };

        await _unitOfWork.Repository<Vehicle>().AddAsync(vehicle, cancellationToken);

        var ownership = new VehicleOwnerHistory
        {
            Vehicle    = vehicle,
            CustomerId = request.CustomerId,
            StartDate  = DateTime.UtcNow
        };

        await _unitOfWork.Repository<VehicleOwnerHistory>().AddAsync(ownership, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<int>.Success(vehicle.Id);
    }
}

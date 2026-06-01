using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboard.Queries;

public class GetClientVehiclesQuery : IRequest<Result<List<ClientVehicleDto>>>
{
    public int UserId { get; set; }
}

public class ClientVehicleDto
{
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public string? ActiveStatus { get; set; }    // null if no active order
    public int? ActiveOrderId { get; set; }
}

public class GetClientVehiclesQueryHandler : IRequestHandler<GetClientVehiclesQuery, Result<List<ClientVehicleDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetClientVehiclesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ClientVehicleDto>>> Handle(GetClientVehiclesQuery request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsActive, cancellationToken);

        if (user?.Person == null)
            return Result<List<ClientVehicleDto>>.Failure("Usuario no encontrado.");

        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.IsActive && c.PersonId == user.Person.Id, cancellationToken);

        if (customer == null)
            return Result<List<ClientVehicleDto>>.Success(new List<ClientVehicleDto>());

        // Get vehicle IDs owned by the customer (current ownership = EndDate is null)
        var vehicleIds = await _db.VehicleOwnerHistories
            .Where(h => h.CustomerId == customer.Id && h.EndDate == null)
            .Select(h => h.VehicleId)
            .ToListAsync(cancellationToken);

        if (!vehicleIds.Any())
            return Result<List<ClientVehicleDto>>.Success(new List<ClientVehicleDto>());

        var vehicles = await _db.Vehicles
            .Include(v => v.Model)
                .ThenInclude(m => m!.Brand)
            .Include(v => v.Color)
            .Include(v => v.ServiceOrders)
                .ThenInclude(so => so.OrderStatus)
            .Where(v => vehicleIds.Contains(v.Id) && v.IsActive)
            .ToListAsync(cancellationToken);

        var dtos = vehicles.Select(v =>
        {
            var activeOrder = v.ServiceOrders
                .FirstOrDefault(so => so.OrderStatus != null &&
                    so.OrderStatus.Name != "Finalizado" &&
                    so.OrderStatus.Name != "Entregado");

            return new ClientVehicleDto
            {
                Id = v.Id,
                Brand = v.Model?.Brand?.BrandName ?? "Desconocida",
                Model = v.Model?.ModelName ?? "Desconocido",
                Year = v.Year,
                Color = v.Color?.ColorName ?? "Desconocido",
                LicensePlate = v.LicensePlate ?? "S/N",
                Mileage = v.Mileage,
                ActiveStatus = activeOrder?.OrderStatus?.Name,
                ActiveOrderId = activeOrder?.Id
            };
        }).ToList();

        return Result<List<ClientVehicleDto>>.Success(dtos);
    }
}

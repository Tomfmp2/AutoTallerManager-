using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboard.Queries;

public class GetClientHistoryQuery : IRequest<Result<ClientHistoryDto>>
{
    public int UserId { get; set; }
}

public class ClientHistoryDto
{
    public List<ClientServiceOrderDto> History { get; set; } = new();
    public List<ClientServiceOrderDto> UpcomingAppointments { get; set; } = new();
}

public class ClientServiceOrderDto
{
    public int Id { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? WorkPerformed { get; set; }
    public string? Mechanic { get; set; }
    public string EntryDate { get; set; } = string.Empty;
    public string? DeliveryDate { get; set; }
    public string? ScheduledDate { get; set; }
}

public class GetClientHistoryQueryHandler : IRequestHandler<GetClientHistoryQuery, Result<ClientHistoryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetClientHistoryQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ClientHistoryDto>> Handle(GetClientHistoryQuery request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsActive, cancellationToken);

        if (user?.Person == null)
            return Result<ClientHistoryDto>.Failure("Usuario no encontrado.");

        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.IsActive && c.PersonId == user.Person.Id, cancellationToken);

        if (customer == null)
            return Result<ClientHistoryDto>.Success(new ClientHistoryDto());

        var vehicleIds = await _db.VehicleOwnerHistories
            .Where(h => h.CustomerId == customer.Id)
            .Select(h => h.VehicleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!vehicleIds.Any())
            return Result<ClientHistoryDto>.Success(new ClientHistoryDto());

        var allOrders = await _db.ServiceOrders
            .Include(so => so.Vehicle)
                .ThenInclude(v => v!.Model)
                    .ThenInclude(m => m!.Brand)
            .Include(so => so.ServiceType)
            .Include(so => so.OrderStatus)
            .Include(so => so.Mechanic)
                .ThenInclude(m => m!.Person)
            .Where(so => vehicleIds.Contains(so.VehicleId))
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync(cancellationToken);

        ClientServiceOrderDto MapOrder(Domain.Entities.ServiceOrder so) => new()
        {
            Id = so.Id,
            ServiceType = so.ServiceType?.Name ?? "Servicio General",
            Vehicle = $"{so.Vehicle?.Model?.Brand?.BrandName} {so.Vehicle?.Model?.ModelName}".Trim(),
            LicensePlate = so.Vehicle?.LicensePlate ?? "S/N",
            Status = so.OrderStatus?.Name ?? "Desconocido",
            WorkPerformed = so.WorkPerformed,
            Mechanic = so.Mechanic?.Person != null
                ? $"{so.Mechanic.Person.FirstName} {so.Mechanic.Person.LastName}"
                : null,
            EntryDate = so.EntryDate.ToString("dd/MM/yyyy"),
            DeliveryDate = (so.ActualDeliveryDate ?? so.EstimatedDeliveryDate)?.ToString("dd/MM/yyyy"),
            ScheduledDate = so.ScheduledDate?.ToString("dd/MM/yyyy HH:mm")
        };

        var completed = new[] { "Finalizado", "Entregado" };

        var dto = new ClientHistoryDto
        {
            History = allOrders
                .Where(so => completed.Contains(so.OrderStatus?.Name ?? ""))
                .Select(MapOrder)
                .ToList(),

            UpcomingAppointments = allOrders
                .Where(so => so.OrderStatus?.Name == "Pendiente" && so.ScheduledDate > DateTime.UtcNow)
                .OrderBy(so => so.ScheduledDate)
                .Select(MapOrder)
                .ToList()
        };

        return Result<ClientHistoryDto>.Success(dto);
    }
}

using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Features.Dashboard.Queries;

public class GetDashboardClientSummaryQuery : IRequest<Result<DashboardClientSummaryDto>>
{
    public int UserId { get; set; }
}

public class DashboardClientSummaryDto
{
    public ActiveRepairDto? ActiveRepair { get; set; }
    public List<AppointmentDto> UpcomingAppointments { get; set; } = new();
    public List<RecentHistoryDto> RecentHistory { get; set; } = new();
}

public class ActiveRepairDto
{
    public string VehicleInfo { get; set; } = string.Empty; // e.g. Toyota Corolla
    public string LicensePlate { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public string Status { get; set; } = string.Empty; // e.g. EN REPARACIÓN
    public string CurrentWork { get; set; } = string.Empty;
    public string? EstimatedDelivery { get; set; }
}

public class AppointmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty; // e.g. Revisión de 10,000 km
    public string Date { get; set; } = string.Empty;
}

public class RecentHistoryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
}

public class GetDashboardClientSummaryQueryHandler : IRequestHandler<GetDashboardClientSummaryQuery, Result<DashboardClientSummaryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetDashboardClientSummaryQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<DashboardClientSummaryDto>> Handle(GetDashboardClientSummaryQuery request, CancellationToken cancellationToken)
    {
        // Find User -> Person -> Customer
        var user = await _db.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsActive, cancellationToken);
            
        if (user?.Person == null)
            return Result<DashboardClientSummaryDto>.Failure("Usuario no tiene perfil asociado.");

        // 1. Get the Customer associated with the PersonId
        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.IsActive && c.PersonId == user.Person.Id, cancellationToken);

        if (customer == null)
        {
            // If the user doesn't have a Customer profile yet, return an empty dashboard
            return Result<DashboardClientSummaryDto>.Success(new DashboardClientSummaryDto());
        }

        // 2. Fetch Customer's active vehicles
        var vehicleIds = await _db.VehicleOwnerHistories
            .Where(h => h.CustomerId == customer.Id && h.EndDate == null)
            .Select(h => h.VehicleId)
            .ToListAsync(cancellationToken);

        // 3. Fetch Service Orders for those vehicles
        var serviceOrders = await _db.ServiceOrders
            .Include(so => so.Vehicle)
                .ThenInclude(v => v.Model)
                    .ThenInclude(m => m.Brand)
            .Include(so => so.OrderStatus)
            .Include(so => so.ServiceType)
            .Where(so => vehicleIds.Contains(so.VehicleId))
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync(cancellationToken);

        var dto = new DashboardClientSummaryDto();

        // Active Repair: First order that is NOT completed (assuming Completed/Delivered are certain states)
        // Here we map OrderStatusId to Progress. Let's assume:
        // 1 = Pendiente (10%), 2 = En Reparación (50%), 3 = Finalizado (100%), 4 = Entregado (100%)
        // We will do a rough map.
        var activeOrder = serviceOrders.FirstOrDefault(so => so.OrderStatus.Name != "Finalizado" && so.OrderStatus.Name != "Entregado");
        if (activeOrder != null)
        {
            int progress = activeOrder.OrderStatus.Name switch
            {
                "Pendiente" => 10,
                "En Reparación" => 50,
                "En Pruebas" => 80,
                "Listo para Entregar" => 90,
                _ => 30
            };

            var brandName = activeOrder.Vehicle.Model?.Brand?.BrandName ?? "Vehículo";
            var modelName = activeOrder.Vehicle.Model?.ModelName ?? "";

            dto.ActiveRepair = new ActiveRepairDto
            {
                VehicleInfo = $"{brandName} {modelName}".Trim(),
                LicensePlate = activeOrder.Vehicle.LicensePlate ?? "S/N",
                ProgressPercentage = progress,
                Status = activeOrder.OrderStatus.Name.ToUpper(),
                CurrentWork = activeOrder.WorkPerformed ?? activeOrder.ServiceType?.Name ?? "Servicio General",
                EstimatedDelivery = activeOrder.EstimatedDeliveryDate?.ToString("dd/MM/yyyy HH:mm") ?? "No asignada"
            };
        }

        // Upcoming Appointments (ScheduledDate > Now and Status == Pendiente)
        var upcoming = serviceOrders
            .Where(so => so.OrderStatus.Name == "Pendiente" && so.ScheduledDate > DateTime.UtcNow)
            .OrderBy(so => so.ScheduledDate)
            .Take(3)
            .ToList();

        dto.UpcomingAppointments = upcoming.Select(so => new AppointmentDto
        {
            Id = so.Id,
            Title = so.ServiceType?.Name ?? "Cita de servicio",
            Date = so.ScheduledDate?.ToString("dd/MM/yyyy") ?? ""
        }).ToList();

        // Recent History (Status == Finalizado or Entregado)
        var history = serviceOrders
            .Where(so => so.OrderStatus.Name == "Finalizado" || so.OrderStatus.Name == "Entregado")
            .OrderByDescending(so => so.ActualDeliveryDate ?? so.LastModifiedAt ?? so.CreatedAt)
            .Take(5)
            .ToList();

        dto.RecentHistory = history.Select(so => new RecentHistoryDto
        {
            Id = so.Id,
            Title = so.ServiceType?.Name ?? "Servicio Completado",
            Description = so.WorkPerformed ?? "Servicio completado satisfactoriamente.",
            Date = (so.ActualDeliveryDate ?? so.CreatedAt).ToString("dd/MM/yyyy")
        }).ToList();

        return Result<DashboardClientSummaryDto>.Success(dto);
    }
}

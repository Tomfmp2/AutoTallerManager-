using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Features.Recepcion.Commands;

public class ApproveOrderCommand : IRequest<Result<bool>>
{
    [JsonIgnore]
    public int ReceptionistId { get; set; }

    public int ServiceOrderId { get; set; }
    public int MechanicId { get; set; }
}

public class ApproveOrderCommandHandler : IRequestHandler<ApproveOrderCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public ApproveOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(ApproveOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.OrderStatus)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId, cancellationToken);

        if (order == null) return Result<bool>.Failure("Orden no encontrada.");
        if (order.OrderStatus?.Name != "Pendiente") return Result<bool>.Failure("La orden no está en estado Pendiente.");

        var mechanic = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == request.MechanicId && u.IsActive, cancellationToken);

        if (mechanic == null || !mechanic.UserRoles.Any(ur => ur.Role.Name == "Mecanico"))
            return Result<bool>.Failure("Mecánico no válido.");

        var programadaStatus = await _context.OrderStatuses
            .FirstOrDefaultAsync(os => os.Name == "Programada", cancellationToken);

        order.MechanicId = request.MechanicId;
        order.ReceptionistId = request.ReceptionistId;
        order.OrderStatusId = programadaStatus!.Id;
        order.LastModifiedAt = DateTime.UtcNow;

        _context.ServiceOrders.Update(order);

        // Notificar al cliente
        var vehicle = await _context.Vehicles
            .Include(v => v.OwnerHistories)
            .FirstOrDefaultAsync(v => v.Id == order.VehicleId, cancellationToken);

        var currentOwner = vehicle?.OwnerHistories.FirstOrDefault(h => h.EndDate == null);
        if (currentOwner != null)
        {
            var customer = await _context.Customers.FindAsync(currentOwner.CustomerId);
            if (customer != null)
            {
                var clientUser = await _context.Users.FirstOrDefaultAsync(u => u.PersonId == customer.PersonId, cancellationToken);
                if (clientUser != null)
                {
                    _context.Notifications.Add(new Domain.Entities.Notification
                    {
                        UserId = clientUser.Id,
                        Title = "Cita Confirmada",
                        Message = $"Tu cita para el vehículo {vehicle!.LicensePlate ?? vehicle.VIN} ha sido confirmada y un mecánico ha sido asignado.",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

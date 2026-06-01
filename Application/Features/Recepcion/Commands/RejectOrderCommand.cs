using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Features.Recepcion.Commands;

public class RejectOrderCommand : IRequest<Result<bool>>
{
    [JsonIgnore]
    public int ReceptionistId { get; set; }

    public int ServiceOrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class RejectOrderCommandHandler : IRequestHandler<RejectOrderCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public RejectOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RejectOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.OrderStatus)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId, cancellationToken);

        if (order == null) return Result<bool>.Failure("Orden no encontrada.");
        if (order.OrderStatus?.Name != "Pendiente") return Result<bool>.Failure("La orden no está en estado Pendiente.");

        var canceladaStatus = await _context.OrderStatuses
            .FirstOrDefaultAsync(os => os.Name == "Cancelada", cancellationToken);

        order.ReceptionistId = request.ReceptionistId;
        order.OrderStatusId = canceladaStatus!.Id;
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
                        Title = "Cita Rechazada",
                        Message = $"Tu solicitud de cita para el vehículo {vehicle!.LicensePlate ?? vehicle.VIN} ha sido rechazada. Motivo: {request.Reason}",
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

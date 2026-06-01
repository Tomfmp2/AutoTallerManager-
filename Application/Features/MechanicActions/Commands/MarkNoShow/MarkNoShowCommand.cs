using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MechanicActions.Commands.MarkNoShow;

public class MarkNoShowCommand : IRequest<Result<bool>>
{
    public int OrderId { get; set; }
}

public class MarkNoShowCommandHandler : IRequestHandler<MarkNoShowCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public MarkNoShowCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(MarkNoShowCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v.OwnerHistories)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null || !order.ScheduledDate.HasValue) 
            return Result<bool>.Failure("Orden de servicio no encontrada o no tiene fecha programada.");

        // Validate time: must be 1 hour or more after scheduled time
        if (DateTime.UtcNow < order.ScheduledDate.Value.AddHours(1))
            return Result<bool>.Failure("Debe pasar al menos 1 hora desde la cita para marcar incumplimiento.");

        var incumplimientoStatus = await _context.OrderStatuses
            .FirstOrDefaultAsync(s => s.Name == "Incumplimiento", cancellationToken);

        if (incumplimientoStatus != null)
        {
            order.OrderStatusId = incumplimientoStatus.Id;
        }

        var currentOwner = order.Vehicle?.OwnerHistories.FirstOrDefault(h => !h.EndDate.HasValue);
        if (currentOwner != null)
        {
            var customer = await _context.Customers.FindAsync(currentOwner.CustomerId);
            if (customer != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.PersonId == customer.PersonId, cancellationToken);
                if (user != null)
                {
                    var notification = new Notification
                    {
                        UserId = user.Id,
                        Title = "Cita Vencida",
                        Message = $"Cita vencida por incumplimiento. No te presentaste a la hora acordada para tu vehículo {order.Vehicle?.LicensePlate}.",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    };
                    _context.Notifications.Add(notification);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

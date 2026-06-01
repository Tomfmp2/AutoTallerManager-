using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MechanicActions.Commands.CompleteOrder;

public class CompleteOrderCommand : IRequest<Result<bool>>
{
    public int OrderId { get; set; }
}

public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CompleteOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v.OwnerHistories)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null) return Result<bool>.Failure("Orden de servicio no encontrada.");

        var completedStatus = await _context.OrderStatuses
            .FirstOrDefaultAsync(s => s.Name == "Completada", cancellationToken);

        if (completedStatus != null)
        {
            order.OrderStatusId = completedStatus.Id;
            order.ActualDeliveryDate = DateTime.UtcNow;
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
                        Title = "¡Vehículo Listo!",
                        Message = $"El trabajo técnico de tu vehículo {order.Vehicle?.LicensePlate} ha finalizado. ¡Está listo para recoger!",
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

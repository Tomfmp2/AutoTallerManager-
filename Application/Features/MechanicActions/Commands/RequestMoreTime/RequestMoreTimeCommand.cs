using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Features.MechanicActions.Commands.RequestMoreTime;

public class RequestMoreTimeCommand : IRequest<Result<bool>>
{
    public int OrderId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class RequestMoreTimeCommandHandler : IRequestHandler<RequestMoreTimeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public RequestMoreTimeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RequestMoreTimeCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v.OwnerHistories)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null) return Result<bool>.Failure("Orden de servicio no encontrada.");

        // Añadir a las observaciones (si se desea)
        order.Observations = string.IsNullOrEmpty(order.Observations) 
            ? request.Message 
            : order.Observations + "\n[Más tiempo solicitado]: " + request.Message;

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
                        Title = "Mecánico solicita más tiempo",
                        Message = $"El mecánico ha solicitado más tiempo para tu vehículo {order.Vehicle?.LicensePlate}. Mensaje: {request.Message}",
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

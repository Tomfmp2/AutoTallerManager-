using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Recepcion.Commands.StartMaintenance;

public class StartMaintenanceCommand : IRequest<Result<int>>
{
    public int ServiceOrderId { get; set; }
    public int EstimatedHours { get; set; }
}

public class StartMaintenanceCommandHandler : IRequestHandler<StartMaintenanceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public StartMaintenanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(StartMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.OrderStatus)
            .Include(o => o.Vehicle)
                .ThenInclude(v => v!.OwnerHistories)
                    .ThenInclude(h => h.Customer)
                        .ThenInclude(c => c!.Person)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId, cancellationToken);

        if (order == null)
            return Result<int>.Failure("Orden de servicio no encontrada.");

        if (order.OrderStatus?.Name != "DiagnosticoCompletado" && order.OrderStatus?.Name != "Programada")
            return Result<int>.Failure("La orden no está en un estado válido para iniciar mantenimiento (se requiere DiagnosticoCompletado).");

        var status = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "EnProceso", cancellationToken);
        if (status != null)
        {
            order.OrderStatusId = status.Id;
        }

        order.EstimatedDeliveryDate = DateTime.UtcNow.AddHours(request.EstimatedHours);

        // Notificar al cliente
        var currentOwner = order.Vehicle?.OwnerHistories.FirstOrDefault(h => h.EndDate == null);
        if (currentOwner?.Customer?.Person != null)
        {
            var customerUserId = await _context.Users
                .Where(u => u.PersonId == currentOwner.Customer.PersonId)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (customerUserId != 0)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = customerUserId,
                    Title = "Mantenimiento Iniciado",
                    Message = $"El diagnóstico fue aprobado. Tu vehículo ha entrado en reparación. Tiempo estimado: {request.EstimatedHours} horas.",
                    IsRead = false
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(order.Id);
    }
}

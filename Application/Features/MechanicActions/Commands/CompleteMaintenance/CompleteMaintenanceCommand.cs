using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MechanicActions.Commands.CompleteMaintenance;

public class CompleteMaintenanceCommand : IRequest<Result<int>>
{
    public int ServiceOrderId { get; set; }
    public int MechanicId { get; set; }
    public string FinalReport { get; set; } = string.Empty;
}

public class CompleteMaintenanceCommandHandler : IRequestHandler<CompleteMaintenanceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CompleteMaintenanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CompleteMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.OrderStatus)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId && o.MechanicId == request.MechanicId, cancellationToken);

        if (order == null)
            return Result<int>.Failure("Orden de servicio no encontrada o no está asignada a este mecánico.");

        if (order.OrderStatus?.Name != "EnProceso")
            return Result<int>.Failure("La orden no está en proceso, no se puede finalizar.");

        var status = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "Completada", cancellationToken);
        if (status != null)
        {
            order.OrderStatusId = status.Id;
        }

        var report = new ServiceOrderReport
        {
            ServiceOrderId = request.ServiceOrderId,
            MechanicId = request.MechanicId,
            ReportText = "Mantenimiento finalizado: " + request.FinalReport,
            IsDiagnostic = false
        };
        _context.ServiceOrderReports.Add(report);

        // Notificar a Recepción
        // Por simplicidad, agregamos una notificacion a todos los recepcionistas activos,
        // o si guardamos el ReceptionistId en ServiceOrder, le notificamos a ese.
        // Dado que ServiceOrder tiene ReceptionistId, notificamos a ese si existe.
        if (order.ReceptionistId.HasValue)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = order.ReceptionistId.Value,
                Title = "Mantenimiento Completado",
                Message = $"El mecánico ha terminado el mantenimiento de la orden #{order.Id}.",
                IsRead = false
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(order.Id);
    }
}

using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Recepcion.Queries.GetReceptionStats;

public class ReceptionStatsDto
{
    public int PendingAppointments { get; set; }
    public int ActiveRepairs { get; set; }
    public int TotalClients { get; set; }
}

public class GetReceptionStatsQuery : IRequest<Result<ReceptionStatsDto>>
{
}

public class GetReceptionStatsQueryHandler : IRequestHandler<GetReceptionStatsQuery, Result<ReceptionStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetReceptionStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ReceptionStatsDto>> Handle(GetReceptionStatsQuery request, CancellationToken cancellationToken)
    {
        // 1. Citas pendientes (ServiceOrders with status "Pendiente")
        var pendingAppointments = await _context.ServiceOrders
            .Include(o => o.OrderStatus)
            .CountAsync(o => o.OrderStatus != null && o.OrderStatus.Name == "Pendiente", cancellationToken);

        // 2. Reparaciones activas (ServiceOrders with status "EnProgreso" or "Diagnosticando")
        var activeRepairs = await _context.ServiceOrders
            .Include(o => o.OrderStatus)
            .CountAsync(o => o.OrderStatus != null && (o.OrderStatus.Name == "EnProgreso" || o.OrderStatus.Name == "Diagnosticando" || o.OrderStatus.Name == "DiagnosticoCompletado"), cancellationToken);

        // 3. Clientes (Count customers)
        var totalClients = await _context.Customers.CountAsync(cancellationToken);

        return Result<ReceptionStatsDto>.Success(new ReceptionStatsDto
        {
            PendingAppointments = pendingAppointments,
            ActiveRepairs = activeRepairs,
            TotalClients = totalClients
        });
    }
}

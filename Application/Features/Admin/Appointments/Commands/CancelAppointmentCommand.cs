using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Admin.Appointments.Commands;

public class CancelAppointmentCommand : IRequest<Result<bool>>
{
    public int OrderId { get; set; }
}

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CancelAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order == null) return Result<bool>.Failure("Cita no encontrada.");

        var canceledStatus = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "Cancelada", cancellationToken);
        if (canceledStatus != null)
        {
            order.OrderStatusId = canceledStatus.Id;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

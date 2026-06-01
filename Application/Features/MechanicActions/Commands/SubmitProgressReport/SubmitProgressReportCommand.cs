using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MechanicActions.Commands.SubmitProgressReport;

public class SubmitProgressReportCommand : IRequest<Result<int>>
{
    public int ServiceOrderId { get; set; }
    public int MechanicId { get; set; }
    public string ReportText { get; set; } = string.Empty;
}

public class SubmitProgressReportCommandHandler : IRequestHandler<SubmitProgressReportCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public SubmitProgressReportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(SubmitProgressReportCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId && o.MechanicId == request.MechanicId, cancellationToken);

        if (order == null)
            return Result<int>.Failure("Orden de servicio no encontrada o no está asignada a este mecánico.");

        var report = new ServiceOrderReport
        {
            ServiceOrderId = request.ServiceOrderId,
            MechanicId = request.MechanicId,
            ReportText = request.ReportText,
            IsDiagnostic = false
        };
        _context.ServiceOrderReports.Add(report);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(report.Id);
    }
}

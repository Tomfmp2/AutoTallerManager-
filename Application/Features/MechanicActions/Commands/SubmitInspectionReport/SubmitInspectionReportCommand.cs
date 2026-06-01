using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MechanicActions.Commands.SubmitInspectionReport;

public class SubmitInspectionReportCommand : IRequest<Result<int>>
{
    public int ServiceOrderId { get; set; }
    public int MechanicId { get; set; }
    public string DiagnosticText { get; set; } = string.Empty;
    public int EstimatedHours { get; set; }
    public List<ReportPartDto> Parts { get; set; } = new();
}

public class ReportPartDto
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}

public class SubmitInspectionReportCommandHandler : IRequestHandler<SubmitInspectionReportCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public SubmitInspectionReportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(SubmitInspectionReportCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.OrderStatus)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId && o.MechanicId == request.MechanicId, cancellationToken);

        if (order == null)
            return Result<int>.Failure("Orden de servicio no encontrada o no está asignada a este mecánico.");

        if (order.OrderStatus?.Name != "Programada")
            return Result<int>.Failure("La orden no está en estado Programada, no se puede enviar diagnóstico.");

        // Crear el reporte
        var report = new ServiceOrderReport
        {
            ServiceOrderId = request.ServiceOrderId,
            MechanicId = request.MechanicId,
            ReportText = request.DiagnosticText,
            IsDiagnostic = true,
            EstimatedHours = request.EstimatedHours
        };
        _context.ServiceOrderReports.Add(report);

        // Add Parts
        if (request.Parts != null && request.Parts.Any())
        {
            var partIds = request.Parts.Select(p => p.PartId).ToList();
            var dbParts = await _context.Parts.Where(p => partIds.Contains(p.Id)).ToListAsync(cancellationToken);

            foreach (var p in request.Parts)
            {
                var dbPart = dbParts.FirstOrDefault(x => x.Id == p.PartId);
                if (dbPart != null)
                {
                    report.ReportParts.Add(new ServiceOrderReportPart
                    {
                        PartId = dbPart.Id,
                        Quantity = p.Quantity,
                        UnitPriceSnapshot = dbPart.UnitPrice
                    });
                }
            }
        }

        // Cambiar estado a DiagnosticoCompletado
        var status = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "DiagnosticoCompletado", cancellationToken);
        if (status != null)
        {
            order.OrderStatusId = status.Id;
        }

        // Enviar notificacion a la recepcionista (asumimos recepcionista que la asigno, pero si no hay ReceptionistId, notificamos general o a admin? Dejamos constancia)
        // Por simplicidad en la UI, el recepcionista vera todas en DiagnosticoCompletado.

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(report.Id);
    }
}

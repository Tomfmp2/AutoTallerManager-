using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Recepcion.Commands.GenerateInvoice;

public class GenerateInvoiceCommand : IRequest<Result<int>>
{
    public int ServiceOrderId { get; set; }
}

public class GenerateInvoiceCommandHandler : IRequestHandler<GenerateInvoiceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public GenerateInvoiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.OrderStatus)
            .Include(o => o.ServiceType)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId, cancellationToken);

        if (order == null)
            return Result<int>.Failure("Orden de servicio no encontrada.");

        if (order.OrderStatus?.Name != "DiagnosticoCompletado")
            return Result<int>.Failure("La orden no está en estado DiagnosticoCompletado.");

        var diagnosticReport = await _context.ServiceOrderReports
            .Include(r => r.ReportParts)
            .Where(r => r.ServiceOrderId == request.ServiceOrderId && r.IsDiagnostic)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (diagnosticReport == null)
            return Result<int>.Failure("No se encontró un reporte de diagnóstico para esta orden.");

        // Calcular costos
        decimal laborCost = (diagnosticReport.EstimatedHours ?? 0) * (order.ServiceType?.PricePerHour ?? 0);
        decimal partsCost = 0;

        var invoice = new Invoice
        {
            WorkshopId = order.WorkshopId,
            ServiceOrderId = order.Id,
            PaymentMethodId = 1, // Por defecto, luego el cliente o recepcionista lo actualizará
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{order.Id}",
            InvoiceDate = DateTime.UtcNow,
            PaymentStatus = "PendienteAprobacion",
            LaborCost = laborCost
        };

        // Agregar detalles de mano de obra
        invoice.Details.Add(new InvoiceDetail
        {
            Concept = $"Mano de obra: {order.ServiceType?.Name} ({diagnosticReport.EstimatedHours} horas)",
            Quantity = 1,
            UnitPrice = laborCost,
            Subtotal = laborCost
        });

        // Agregar detalles de repuestos
        foreach (var rp in diagnosticReport.ReportParts)
        {
            var part = await _context.Parts.FindAsync(new object[] { rp.PartId }, cancellationToken);
            if (part != null)
            {
                decimal rpTotal = rp.UnitPriceSnapshot * rp.Quantity;
                partsCost += rpTotal;
                
                invoice.Details.Add(new InvoiceDetail
                {
                    Concept = $"Repuesto: {part.Description}",
                    Quantity = rp.Quantity,
                    UnitPrice = rp.UnitPriceSnapshot,
                    Subtotal = rpTotal
                });
            }
        }

        invoice.Subtotal = laborCost + partsCost;
        invoice.Taxes = invoice.Subtotal * 0.19m; // Ejemplo 19% IVA
        invoice.Total = invoice.Subtotal + invoice.Taxes;

        _context.Invoices.Add(invoice);

        // Cambiar estado
        var newStatus = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "EsperandoAprobacionCliente", cancellationToken);
        if (newStatus != null)
        {
            order.OrderStatusId = newStatus.Id;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(invoice.Id);
    }
}

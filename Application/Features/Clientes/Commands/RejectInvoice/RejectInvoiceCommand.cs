using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clientes.Commands.RejectInvoice;

public class RejectInvoiceCommand : IRequest<Result<int>>
{
    public int InvoiceId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class RejectInvoiceCommandHandler : IRequestHandler<RejectInvoiceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public RejectInvoiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(RejectInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.ServiceOrder)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice == null)
            return Result<int>.Failure("Factura no encontrada.");

        if (invoice.ServiceOrder == null)
            return Result<int>.Failure("Orden de servicio asociada no encontrada.");

        invoice.PaymentStatus = "Rechazada";

        // Regresar la orden a DiagnosticoCompletado para que la recepcionista / mecanico la ajusten
        var newStatus = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "DiagnosticoCompletado", cancellationToken);
        if (newStatus != null)
        {
            invoice.ServiceOrder.OrderStatusId = newStatus.Id;
        }

        // Podríamos guardar el motivo en Observations o en un historial
        invoice.ServiceOrder.Observations += $"\nRechazo Cliente: {request.Reason}";

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(invoice.Id);
    }
}

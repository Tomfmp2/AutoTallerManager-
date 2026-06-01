using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Recepcion.Commands.ConfirmPayment;

public class ConfirmPaymentCommand : IRequest<Result<int>>
{
    public int InvoiceId { get; set; }
}

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public ConfirmPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.ServiceOrder)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice == null)
            return Result<int>.Failure("Factura no encontrada.");

        if (invoice.ServiceOrder == null)
            return Result<int>.Failure("Orden de servicio asociada no encontrada.");

        if (invoice.PaymentStatus != "AprobadaPorCliente")
            return Result<int>.Failure("La factura no está aprobada por el cliente.");

        // Recepcionista confirma el pago
        invoice.PaymentStatus = "Pagada"; // o EnProcesoDeMantenimiento, etc.

        var newStatus = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "EnProceso", cancellationToken);
        if (newStatus != null)
        {
            invoice.ServiceOrder.OrderStatusId = newStatus.Id;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(invoice.Id);
    }
}

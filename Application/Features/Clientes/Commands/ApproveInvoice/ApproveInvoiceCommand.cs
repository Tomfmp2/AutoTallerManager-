using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clientes.Commands.ApproveInvoice;

public class ApproveInvoiceCommand : IRequest<Result<int>>
{
    public int InvoiceId { get; set; }
}

public class ApproveInvoiceCommandHandler : IRequestHandler<ApproveInvoiceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public ApproveInvoiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(ApproveInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.ServiceOrder)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice == null)
            return Result<int>.Failure("Factura no encontrada.");

        if (invoice.ServiceOrder == null)
            return Result<int>.Failure("Orden de servicio asociada no encontrada.");

        // Cliente aprueba, cambiamos estado a EsperandoPago
        invoice.PaymentStatus = "AprobadaPorCliente";

        var newStatus = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "EsperandoPago", cancellationToken);
        if (newStatus != null)
        {
            invoice.ServiceOrder.OrderStatusId = newStatus.Id;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(invoice.Id);
    }
}

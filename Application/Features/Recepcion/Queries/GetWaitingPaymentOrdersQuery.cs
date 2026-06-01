using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Recepcion.Queries;

public class GetWaitingPaymentOrdersQuery : IRequest<Result<List<WaitingPaymentOrderDto>>>
{
}

public class WaitingPaymentOrderDto
{
    public int Id { get; set; }
    public string ServiceType { get; set; } = null!;
    public string Vehicle { get; set; } = null!;
    public string LicensePlate { get; set; } = null!;
    public string? ClientName { get; set; }
    public DateTime EntryDate { get; set; }
    public int? InvoiceId { get; set; }
    public decimal? Total { get; set; }
    public string? PaymentStatus { get; set; }
}

public class GetWaitingPaymentOrdersQueryHandler : IRequestHandler<GetWaitingPaymentOrdersQuery, Result<List<WaitingPaymentOrderDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetWaitingPaymentOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<WaitingPaymentOrderDto>>> Handle(GetWaitingPaymentOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v!.OwnerHistories)
                    .ThenInclude(h => h.Customer)
                        .ThenInclude(c => c!.Person)
            .Include(o => o.ServiceType)
            .Include(o => o.OrderStatus)
            .Where(o => o.OrderStatus!.Name == "EsperandoPago" || o.OrderStatus!.Name == "EsperandoAprobacionCliente")
            .OrderBy(o => o.EntryDate)
            .ToListAsync(cancellationToken);

        var result = new List<WaitingPaymentOrderDto>();

        foreach (var o in orders)
        {
            var invoice = await _context.Invoices
                .Where(i => i.ServiceOrderId == o.Id)
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var owner = o.Vehicle?.OwnerHistories?.FirstOrDefault();
            var clientName = owner?.Customer?.Person != null
                ? $"{owner.Customer.Person.FirstName} {owner.Customer.Person.LastName}"
                : null;

            result.Add(new WaitingPaymentOrderDto
            {
                Id = o.Id,
                ServiceType = o.ServiceType?.Name ?? "N/A",
                Vehicle = $"{o.Vehicle?.Model?.Brand?.BrandName} {o.Vehicle?.Model?.ModelName}",
                LicensePlate = o.Vehicle?.LicensePlate ?? "N/A",
                ClientName = clientName,
                EntryDate = o.EntryDate,
                InvoiceId = invoice?.Id,
                Total = invoice?.Total,
                PaymentStatus = invoice?.PaymentStatus
            });
        }

        return Result<List<WaitingPaymentOrderDto>>.Success(result);
    }
}

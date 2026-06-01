using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Facturas.Queries.GetClientInvoices;

public class GetClientInvoicesQuery : IRequest<Result<List<ClientInvoiceDto>>>
{
    public int UserId { get; set; }
}

public class ClientInvoiceDetailDto
{
    public string Concept { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class ClientInvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Taxes { get; set; }
    public decimal Total { get; set; }
    public string PaymentStatus { get; set; } = null!;
    public int ServiceOrderId { get; set; }
    public List<ClientInvoiceDetailDto> Details { get; set; } = new();
}

public class GetClientInvoicesQueryHandler : IRequestHandler<GetClientInvoicesQuery, Result<List<ClientInvoiceDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetClientInvoicesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ClientInvoiceDto>>> Handle(GetClientInvoicesQuery request, CancellationToken cancellationToken)
    {
        // Get User -> Person
        var user = await _db.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsActive, cancellationToken);

        if (user?.Person == null)
            return Result<List<ClientInvoiceDto>>.Success(new());

        // Get Customer via PersonId
        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.IsActive && c.PersonId == user.Person.Id, cancellationToken);

        if (customer == null)
            return Result<List<ClientInvoiceDto>>.Success(new());

        // Get vehicles owned by customer
        var vehicleIds = await _db.VehicleOwnerHistories
            .Where(h => h.CustomerId == customer.Id)
            .Select(h => h.VehicleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!vehicleIds.Any())
            return Result<List<ClientInvoiceDto>>.Success(new());

        // Get service orders for those vehicles
        var orderIds = await _db.ServiceOrders
            .Where(so => vehicleIds.Contains(so.VehicleId))
            .Select(so => so.Id)
            .ToListAsync(cancellationToken);

        if (!orderIds.Any())
            return Result<List<ClientInvoiceDto>>.Success(new());

        // Get invoices for those orders
        var invoices = await _db.Invoices
            .Include(i => i.Details)
            .Where(i => orderIds.Contains(i.ServiceOrderId))
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);

        var result = invoices.Select(inv => new ClientInvoiceDto
        {
            Id = inv.Id,
            InvoiceNumber = inv.InvoiceNumber,
            InvoiceDate = inv.InvoiceDate,
            Subtotal = inv.Subtotal,
            Taxes = inv.Taxes,
            Total = inv.Total,
            PaymentStatus = inv.PaymentStatus ?? "Desconocido",
            ServiceOrderId = inv.ServiceOrderId,
            Details = inv.Details?.Select(d => new ClientInvoiceDetailDto
            {
                Concept = d.Concept,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal
            }).ToList() ?? new()
        }).ToList();

        return Result<List<ClientInvoiceDto>>.Success(result);
    }
}

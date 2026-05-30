using Application.Abstractions;
using Application.Common;
using Application.Features.Facturas.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Facturas.Commands.GenerarFactura;

public class GenerarFacturaCommand : IRequest<Result<int>>
{
    public int WorkshopId      { get; set; }
    public int OrdenServicioId { get; set; }
    public int MetodoPagoId    { get; set; }
    public decimal ManodeObra  { get; set; }
    public decimal IVAPorcentaje { get; set; } = 0.19m; // 19% por defecto Colombia
}

public class GenerarFacturaCommandHandler : IRequestHandler<GenerarFacturaCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _db;

    public GenerarFacturaCommandHandler(IUnitOfWork unitOfWork, IApplicationDbContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public async Task<Result<int>> Handle(GenerarFacturaCommand request, CancellationToken cancellationToken)
    {
        // Verificar que la orden no tenga ya factura
        var already = await _db.Invoices
            .AnyAsync(i => i.ServiceOrderId == request.OrdenServicioId, cancellationToken);
        if (already)
            return Result<int>.Failure("Esta orden de servicio ya tiene una factura generada.");

        // Calcular monto de repuestos usados en la orden
        var parts = await _db.ServiceOrderParts
            .Where(sp => sp.ServiceOrderId == request.OrdenServicioId)
            .ToListAsync(cancellationToken);

        var montoRepuestos = parts.Sum(p => p.Quantity * p.AppliedUnitPrice);
        var subtotal = montoRepuestos + request.ManodeObra;
        var iva      = subtotal * request.IVAPorcentaje;
        var total    = subtotal + iva;

        var invoice = new Invoice
        {
            WorkshopId      = request.WorkshopId,
            ServiceOrderId  = request.OrdenServicioId,
            PaymentMethodId = request.MetodoPagoId,
            InvoiceNumber   = $"FAC-{DateTime.UtcNow:yyyyMMdd}-{request.OrdenServicioId:D6}",
            InvoiceDate     = DateTime.UtcNow,
            LaborCost       = request.ManodeObra,
            Subtotal        = subtotal,
            Taxes           = iva,
            Total           = total,
            PaymentStatus   = "Pendiente",
            Details         = parts.Select(p => new InvoiceDetail
            {
                Concept   = $"Repuesto ID {p.PartId}",
                Quantity  = p.Quantity,
                UnitPrice = p.AppliedUnitPrice,
                Subtotal  = p.Quantity * p.AppliedUnitPrice
            }).ToList()
        };

        await _unitOfWork.Repository<Invoice>().AddAsync(invoice, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<int>.Success(invoice.Id);
    }
}

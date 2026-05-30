using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.OrdenesServicio.Commands.UpdateOrdenServicio;

public class UpdateOrdenServicioCommand : IRequest<Result>
{
    public int Id                     { get; set; }
    public int ServiceTypeId          { get; set; }
    public int MechanicId             { get; set; }
    public int OrderStatusId          { get; set; }
    public DateTime? FechaEstimada    { get; set; }
    public string? Observaciones      { get; set; }
    public string? TrabajoRealizado   { get; set; }
}

public class UpdateOrdenServicioCommandHandler : IRequestHandler<UpdateOrdenServicioCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateOrdenServicioCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateOrdenServicioCommand request, CancellationToken cancellationToken)
    {
        var order = await _db.ServiceOrders
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
            return Result.Failure($"No se encontró la orden de servicio con ID {request.Id}.");

        order.ServiceTypeId           = request.ServiceTypeId;
        order.MechanicId              = request.MechanicId;
        order.OrderStatusId           = request.OrderStatusId;
        order.EstimatedDeliveryDate   = request.FechaEstimada;
        order.Observations            = request.Observaciones;
        order.WorkPerformed           = request.TrabajoRealizado;

        // Si se marca como entregado, registrar fecha real
        if (request.OrderStatusId == 4) // asumiendo 4 = Entregado
            order.ActualDeliveryDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

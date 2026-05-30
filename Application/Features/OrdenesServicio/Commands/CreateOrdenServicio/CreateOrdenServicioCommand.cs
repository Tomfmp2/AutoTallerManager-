using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.OrdenesServicio.Commands.CreateOrdenServicio;

public class CreateOrdenServicioCommand : IRequest<Result<int>>
{
    public int WorkshopId             { get; set; }
    public int VehicleId              { get; set; }
    public int ServiceTypeId          { get; set; }
    public int MechanicId             { get; set; }
    public int? ReceptionistId        { get; set; }
    public int OrderStatusId          { get; set; } = 1; // default: Recibido
    public DateTime? FechaEstimada    { get; set; }
    public string? Observaciones      { get; set; }
}

public class CreateOrdenServicioCommandHandler : IRequestHandler<CreateOrdenServicioCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrdenServicioCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateOrdenServicioCommand request, CancellationToken cancellationToken)
    {
        var order = new ServiceOrder
        {
            WorkshopId             = request.WorkshopId,
            VehicleId              = request.VehicleId,
            ServiceTypeId          = request.ServiceTypeId,
            MechanicId             = request.MechanicId,
            ReceptionistId         = request.ReceptionistId,
            OrderStatusId          = request.OrderStatusId,
            EntryDate              = DateTime.UtcNow,
            EstimatedDeliveryDate  = request.FechaEstimada,
            Observations           = request.Observaciones
        };

        await _unitOfWork.Repository<ServiceOrder>().AddAsync(order, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<int>.Success(order.Id);
    }
}

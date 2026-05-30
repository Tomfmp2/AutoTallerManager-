using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Repuestos.Commands.CreateRepuesto;

public class CreateRepuestoCommandHandler : IRequestHandler<CreateRepuestoCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRepuestoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateRepuestoCommand request, CancellationToken cancellationToken)
    {
        var part = new Part
        {
            WorkshopId      = request.WorkshopId,
            PartCategoryId  = request.PartCategoryId,
            Code            = request.Codigo,
            Description     = request.Descripcion,
            UnitPrice       = request.PrecioUnitario,
            Stock           = request.StockInicial,
            MinimumStock    = request.StockMinimo,
            StorageLocation = request.Ubicacion,
            PartBrand       = request.Marca,
            IsActive        = true
        };

        await _unitOfWork.Repository<Part>().AddAsync(part, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<int>.Success(part.Id);
    }
}

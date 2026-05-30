using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Repuestos.Commands.UpdateRepuesto;

public class UpdateRepuestoCommandHandler : IRequestHandler<UpdateRepuestoCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateRepuestoCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateRepuestoCommand request, CancellationToken cancellationToken)
    {
        var part = await _db.Parts
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsActive, cancellationToken);

        if (part is null)
            return Result.Failure($"No se encontró el repuesto con ID {request.Id}.");

        part.Description     = request.Descripcion;
        part.UnitPrice       = request.PrecioUnitario;
        part.MinimumStock    = request.StockMinimo;
        part.StorageLocation = request.Ubicacion;
        part.PartBrand       = request.Marca;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

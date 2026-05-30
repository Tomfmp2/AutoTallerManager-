using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Repuestos.Commands.DeleteRepuesto;

public class DeleteRepuestoCommand : IRequest<Result>
{
    public int Id { get; set; }
}

public class DeleteRepuestoCommandHandler : IRequestHandler<DeleteRepuestoCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteRepuestoCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeleteRepuestoCommand request, CancellationToken cancellationToken)
    {
        var part = await _db.Parts
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsActive, cancellationToken);

        if (part is null)
            return Result.Failure($"No se encontró el repuesto con ID {request.Id}.");

        part.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

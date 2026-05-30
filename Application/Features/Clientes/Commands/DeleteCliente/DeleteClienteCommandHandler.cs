using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clientes.Commands.DeleteCliente;

public class DeleteClienteCommandHandler : IRequestHandler<DeleteClienteCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteClienteCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive, cancellationToken);

        if (customer is null)
            return Result.Failure($"No se encontró el cliente con ID {request.Id}.");

        customer.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

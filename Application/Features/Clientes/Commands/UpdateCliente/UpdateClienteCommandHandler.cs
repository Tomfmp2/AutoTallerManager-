using Application.Abstractions;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clientes.Commands.UpdateCliente;

public class UpdateClienteCommandHandler : IRequestHandler<UpdateClienteCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateClienteCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers
            .Include(c => c.Person)
                .ThenInclude(p => p!.Phones)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive, cancellationToken);

        if (customer is null)
            return Result.Failure($"No se encontró el cliente con ID {request.Id}.");

        var names = request.Nombre.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        customer.Person!.FirstName = names.Length > 0 ? names[0] : customer.Person.FirstName;
        customer.Person.LastName  = names.Length > 1 ? names[1] : string.Empty;

        var primaryPhone = customer.Person.Phones.FirstOrDefault(p => p.IsPrimary)
                        ?? customer.Person.Phones.FirstOrDefault();
        if (primaryPhone is not null)
            primaryPhone.PhoneNumber = request.Telefono;

        customer.AddressStreet = request.Direccion;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

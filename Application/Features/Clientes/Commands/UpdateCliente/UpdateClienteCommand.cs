using Application.Common;
using MediatR;

namespace Application.Features.Clientes.Commands.UpdateCliente;

public class UpdateClienteCommand : IRequest<Result>
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string? Direccion { get; set; }
}

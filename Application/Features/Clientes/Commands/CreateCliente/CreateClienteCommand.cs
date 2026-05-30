using Application.Common;
using MediatR;

namespace Application.Features.Clientes.Commands.CreateCliente;

public class CreateClienteCommand : IRequest<Result<int>>
{
    public int WorkshopId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string? Direccion { get; set; }
}

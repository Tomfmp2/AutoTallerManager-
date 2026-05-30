using Application.Common;
using MediatR;

namespace Application.Features.Clientes.Commands.DeleteCliente;

public class DeleteClienteCommand : IRequest<Result>
{
    public int Id { get; set; }
}

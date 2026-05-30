using FluentValidation;

namespace Application.Features.Clientes.Commands.UpdateCliente;

public class UpdateClienteCommandValidator : AbstractValidator<UpdateClienteCommand>
{
    public UpdateClienteCommandValidator()
    {
        RuleFor(v => v.Id)
            .GreaterThan(0).WithMessage("El ID del cliente es inválido.");

        RuleFor(v => v.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.");

        RuleFor(v => v.Telefono)
            .NotEmpty().WithMessage("El teléfono es requerido.")
            .MaximumLength(20).WithMessage("El teléfono no debe exceder los 20 caracteres.");
    }
}

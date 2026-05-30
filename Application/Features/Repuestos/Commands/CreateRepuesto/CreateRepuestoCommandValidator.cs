using FluentValidation;

namespace Application.Features.Repuestos.Commands.CreateRepuesto;

public class CreateRepuestoCommandValidator : AbstractValidator<CreateRepuestoCommand>
{
    public CreateRepuestoCommandValidator()
    {
        RuleFor(v => v.Codigo)
            .NotEmpty().WithMessage("El código es requerido.")
            .MaximumLength(50).WithMessage("El código no debe exceder 50 caracteres.");

        RuleFor(v => v.Descripcion)
            .NotEmpty().WithMessage("La descripción es requerida.");

        RuleFor(v => v.PrecioUnitario)
            .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a cero.");

        RuleFor(v => v.StockInicial)
            .GreaterThanOrEqualTo(0).WithMessage("El stock inicial no puede ser negativo.");

        RuleFor(v => v.PartCategoryId)
            .GreaterThan(0).WithMessage("Debe seleccionar una categoría válida.");
    }
}

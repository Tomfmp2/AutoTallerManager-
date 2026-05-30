using FluentValidation;

namespace Application.Features.Vehiculos.Commands.CreateVehiculo;

public class CreateVehiculoCommandValidator : AbstractValidator<CreateVehiculoCommand>
{
    public CreateVehiculoCommandValidator()
    {
        RuleFor(v => v.VIN)
            .NotEmpty().WithMessage("El VIN es requerido.")
            .Length(17).WithMessage("El VIN debe tener exactamente 17 caracteres.");

        RuleFor(v => v.ModelId)
            .GreaterThan(0).WithMessage("Debe seleccionar un modelo de vehículo válido.");

        RuleFor(v => v.ColorId)
            .GreaterThan(0).WithMessage("Debe seleccionar un color válido.");

        RuleFor(v => v.Anio)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1)
            .WithMessage($"El año debe estar entre 1900 y {DateTime.UtcNow.Year + 1}.");

        RuleFor(v => v.Kilometraje)
            .GreaterThanOrEqualTo(0).WithMessage("El kilometraje no puede ser negativo.");

        RuleFor(v => v.CustomerId)
            .GreaterThan(0).WithMessage("Debe asociar el vehículo a un cliente.");
    }
}

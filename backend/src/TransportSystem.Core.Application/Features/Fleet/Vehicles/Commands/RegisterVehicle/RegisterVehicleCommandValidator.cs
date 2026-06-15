using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterVehicle
{
    public class RegisterVehicleCommandValidator : AbstractValidator<RegisterVehicleCommand>
    {
        public RegisterVehicleCommandValidator()
        {
            RuleFor(x => x.LicensePlate)
                .NotEmpty().WithMessage("La matrícula es requerida.")
                .MaximumLength(8).WithMessage("La matrícula no puede superar 8 caracteres.");

            RuleFor(x => x.Capacity)
                .InclusiveBetween(1, 100).WithMessage("La capacidad debe estar entre 1 y 100 pasajeros.");

            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("La marca es requerida.")
                .MaximumLength(100);

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("El modelo es requerido.")
                .MaximumLength(100);

            RuleFor(x => x.Year)
                .InclusiveBetween(1990, DateTime.UtcNow.Year + 1)
                .WithMessage($"El año debe estar entre 1990 y {DateTime.UtcNow.Year + 1}.");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("El color es requerido.")
                .MaximumLength(50);

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("El tipo de vehículo no es válido.");
        }
    }
}

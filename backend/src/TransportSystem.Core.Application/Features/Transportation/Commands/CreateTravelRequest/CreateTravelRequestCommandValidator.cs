using FluentValidation;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CreateTravelRequest
{
    public class CreateTravelRequestCommandValidator : AbstractValidator<CreateTravelRequestCommand>
    {
        public CreateTravelRequestCommandValidator()
        {
            RuleFor(x => x.RequestingArea)
                .NotEmpty().WithMessage("El área solicitante es requerida.")
                .MaximumLength(150).WithMessage("El área no puede superar 150 caracteres.");

            RuleFor(x => x.PassengerCount)
                .GreaterThan(0).WithMessage("La cantidad de colaboradores debe ser al menos 1.")
                .LessThanOrEqualTo(100).WithMessage("La cantidad no puede superar 100 personas.");

            RuleFor(x => x.Destination)
                .NotEmpty().WithMessage("El destino es requerido.")
                .MaximumLength(300).WithMessage("El destino no puede superar 300 caracteres.");

            RuleFor(x => x.DepartureDateTime)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("La fecha de salida debe ser en el futuro.");

            RuleFor(x => x.ReturnDateTime)
                .GreaterThan(x => x.DepartureDateTime)
                .WithMessage("La hora de regreso debe ser posterior a la hora de salida.");

            RuleFor(x => x.TripPurpose)
                .NotEmpty().WithMessage("El motivo del viaje es requerido.")
                .MaximumLength(500).WithMessage("El motivo no puede superar 500 caracteres.");
        }
    }
}

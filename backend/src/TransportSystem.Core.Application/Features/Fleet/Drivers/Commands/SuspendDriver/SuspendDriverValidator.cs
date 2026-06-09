using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.SuspendDriver
{
    public class SuspendDriverValidator : AbstractValidator<SuspendDriverCommand>
    {
        public SuspendDriverValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty()
                .WithMessage("El motivo de la suspensión es requerido.")
                .MaximumLength(500);
        }
    }
}

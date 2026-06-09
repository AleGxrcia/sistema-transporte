using FluentValidation;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CompleteTrip
{
    public class CompleteTripCommandValidator : AbstractValidator<CompleteTripCommand>
    {
        public CompleteTripCommandValidator()
        {
            RuleFor(x => x.RequestId).NotEmpty();
            RuleFor(x => x.ActualReturnTime)
                .GreaterThan(x => x.ActualDepartureTime)
                .WithMessage("El tiempo de regreso real debe ser posterior al de salida real.");
        }
    }
}

using FluentValidation;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CancelRequest
{
    public class CancelRequestCommandValidator : AbstractValidator<CancelRequestCommand>
    {
        public CancelRequestCommandValidator()
        {
            RuleFor(x => x.RequestId).NotEmpty();
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("El motivo de la cancelación es requerido.")
                .MaximumLength(500);
        }
    }
}

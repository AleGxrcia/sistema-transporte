using FluentValidation;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.RejectRequest
{
    public class RejectRequestCommandValidator : AbstractValidator<RejectRequestCommand>
    {
        public RejectRequestCommandValidator()
        {
            RuleFor(x => x.RequestId).NotEmpty();
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("El motivo del rechazo es requerido.")
                .MaximumLength(500);
        }
    }
}

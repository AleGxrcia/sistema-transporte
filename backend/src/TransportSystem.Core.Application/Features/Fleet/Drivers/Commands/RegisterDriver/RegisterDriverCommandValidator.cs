using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RegisterDriver
{
    public class RegisterDriverCommandValidator : AbstractValidator<RegisterDriverCommand>
    {
        public RegisterDriverCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("La cédula es requerida.")
                .Matches(@"^\d{11}$").WithMessage("La cédula debe tener exactamente 11 dígitos.");

            RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LicenseCategory).IsInEnum();

            RuleFor(x => x.LicenseExpirationDate)
                .GreaterThan(DateTime.UtcNow.Date)
                .WithMessage("La licencia debe estar vigente al registrar el conductor.");

            RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Address).MaximumLength(300).When(x => x.Address is not null);
        }
    }
}

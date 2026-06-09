using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RenewLicense
{
    public class RenewLicenseValidator : AbstractValidator<RenewLicenseCommand>
    {
        public RenewLicenseValidator()
        {
            RuleFor(x => x.DriverId).NotEmpty();
            RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LicenseCategory).IsInEnum();
            RuleFor(x => x.ExpirationDate)
                .GreaterThan(DateTime.UtcNow.Date)
                .WithMessage("La nueva licencia debe tener fecha de vencimiento en el futuro.");
        }
    }
}

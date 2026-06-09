using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterMaintenance
{
    public class RegisterMaintenanceCommandValidator : AbstractValidator<RegisterMaintenanceCommand>
    {
        public RegisterMaintenanceCommandValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Workshop).NotEmpty().MaximumLength(200);
            RuleFor(x => x.EntryDate).NotEmpty();
            RuleFor(x => x.Type).IsInEnum().WithMessage("Tipo de mantenimiento inválido.");
            RuleFor(x => x.NextMaintenanceKmScheduled)
                .GreaterThan(0).When(x => x.NextMaintenanceKmScheduled.HasValue)
                .WithMessage("El kilometraje de próximo mantenimiento debe ser mayor a 0.");
        }
    }
}

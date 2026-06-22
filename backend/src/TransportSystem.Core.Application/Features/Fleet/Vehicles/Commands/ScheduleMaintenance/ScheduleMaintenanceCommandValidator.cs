using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ScheduleMaintenance
{
    public class ScheduleMaintenanceCommandValidator : AbstractValidator<ScheduleMaintenanceCommand>
    {
        public ScheduleMaintenanceCommandValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Workshop).MaximumLength(200);
            RuleFor(x => x.ScheduledDate).NotEmpty();
            RuleFor(x => x.Type).IsInEnum().WithMessage("Tipo de mantenimiento inválido.");
            RuleFor(x => x.ScheduledKm)
                .GreaterThan(0).When(x => x.ScheduledKm.HasValue)
                .WithMessage("El kilometraje programado debe ser mayor a 0.");
        }
    }
}

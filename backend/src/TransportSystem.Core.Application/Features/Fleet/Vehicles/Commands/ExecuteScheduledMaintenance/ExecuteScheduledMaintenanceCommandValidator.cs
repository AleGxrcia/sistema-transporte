using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ExecuteScheduledMaintenance
{
    public class ExecuteScheduledMaintenanceCommandValidator : AbstractValidator<ExecuteScheduledMaintenanceCommand>
    {
        public ExecuteScheduledMaintenanceCommandValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();
            RuleFor(x => x.ScheduledMaintenanceId).NotEmpty();
            RuleFor(x => x.EntryDate).NotEmpty();
            RuleFor(x => x.Workshop).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NextMaintenanceKmScheduled)
                .GreaterThan(0).When(x => x.NextMaintenanceKmScheduled.HasValue)
                .WithMessage("El kilometraje de próximo mantenimiento debe ser mayor a 0.");
        }
    }
}

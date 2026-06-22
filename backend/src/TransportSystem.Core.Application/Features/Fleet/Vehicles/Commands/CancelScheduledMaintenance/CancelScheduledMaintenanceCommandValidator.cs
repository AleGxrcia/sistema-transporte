using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.CancelScheduledMaintenance
{
    public class CancelScheduledMaintenanceCommandValidator : AbstractValidator<CancelScheduledMaintenanceCommand>
    {
        public CancelScheduledMaintenanceCommandValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();
            RuleFor(x => x.ScheduledMaintenanceId).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        }
    }
}

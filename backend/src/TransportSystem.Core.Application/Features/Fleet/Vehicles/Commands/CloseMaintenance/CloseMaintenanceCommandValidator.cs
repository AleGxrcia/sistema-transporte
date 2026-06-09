using FluentValidation;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.CloseMaintenance;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterDriver
{
    public class CloseMaintenanceRecordCommandValidator : AbstractValidator<CloseMaintenanceRecordCommand>
    {
        public CloseMaintenanceRecordCommandValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();
            RuleFor(x => x.MaintenanceRecordId).NotEmpty();
            RuleFor(x => x.ActualExitDate).NotEmpty();
            RuleFor(x => x.Cost).GreaterThanOrEqualTo(0)
                .WithMessage("El costo no puede ser negativo.");
        }
    }
}

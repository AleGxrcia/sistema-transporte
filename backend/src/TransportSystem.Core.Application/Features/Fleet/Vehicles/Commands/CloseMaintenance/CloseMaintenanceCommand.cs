using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.CloseMaintenance
{
    public record CloseMaintenanceRecordCommand(
        Guid VehicleId,
        Guid MaintenanceRecordId,
        DateTime ActualExitDate,
        decimal Cost
    ) : IRequest;
}

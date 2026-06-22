using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.CancelScheduledMaintenance
{
    public record CancelScheduledMaintenanceCommand(
        Guid VehicleId,
        Guid ScheduledMaintenanceId,
        string Reason
    ) : IRequest;
}

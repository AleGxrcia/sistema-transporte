using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ExecuteScheduledMaintenance
{
    public record ExecuteScheduledMaintenanceCommand(
        Guid VehicleId,
        Guid ScheduledMaintenanceId,
        DateTime EntryDate,
        string Workshop,
        DateTime? EstimatedExitDate,
        DateTime? NextMaintenanceDateScheduled,
        decimal? NextMaintenanceKmScheduled
    ) : IRequest<Guid>;
}

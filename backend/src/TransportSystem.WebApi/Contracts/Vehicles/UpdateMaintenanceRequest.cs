using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.WebApi.Contracts.Vehicles
{
    public record UpdateMaintenanceRequest(
        MaintenanceType Type,
        string Description,
        DateTime EntryDate,
        string Workshop,
        DateTime? EstimatedExitDate,
        DateTime? NextMaintenanceDateScheduled,
        decimal? NextMaintenanceKmScheduled
    );
}

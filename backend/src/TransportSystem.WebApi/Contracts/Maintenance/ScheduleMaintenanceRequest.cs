using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.WebApi.Contracts.Maintenance
{
    public record ScheduleMaintenanceRequest(
        Guid VehicleId,
        MaintenanceType Type,
        string Description,
        DateTime ScheduledDate,
        string? Workshop,
        decimal? ScheduledKm
    );
}

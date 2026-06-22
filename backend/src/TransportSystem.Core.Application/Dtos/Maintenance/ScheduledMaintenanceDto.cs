namespace TransportSystem.Core.Application.Dtos.Maintenance
{
    public record ScheduledMaintenanceDto(
        Guid Id,
        Guid VehicleId,
        string VehiclePlate,
        string VehicleLabel,
        string Type,
        string Description,
        DateTime ScheduledDate,
        string? Workshop,
        decimal? ScheduledKm,
        string Status,
        int DaysRemaining
    );
}

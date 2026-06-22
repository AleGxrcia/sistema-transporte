namespace TransportSystem.Core.Application.Dtos.Maintenance
{
    public record MaintenanceHistoryItemDto(
        Guid Id,
        Guid VehicleId,
        string VehiclePlate,
        string VehicleLabel,
        string Type,
        string Description,
        DateTime EntryDate,
        DateTime? EstimatedExitDate,
        DateTime? ActualExitDate,
        decimal? Cost,
        string Workshop,
        bool IsClosed,
        DateTime? NextMaintenanceDateScheduled
    );
}

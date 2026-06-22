namespace TransportSystem.Core.Application.Dtos.Maintenance
{
    public record MaintenanceAlertDto(
        Guid VehicleId,
        string VehicleLabel,
        string VehiclePlate,
        string Type,
        DateTime DueDate,
        int DaysRemaining,
        string Source
    );
}

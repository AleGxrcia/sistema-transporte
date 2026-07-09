namespace TransportSystem.Core.Application.Dtos.Vehicle
{
    public record VehicleDto(
        Guid Id,
        string LicensePlate,
        string Brand,
        string Model,
        int Year,
        string Color,
        string Type,
        int Capacity,
        string Status,
        decimal CurrentMileage,
        DateTime? LastMaintenanceDate,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        bool IsDeleted,
        DateTime? DeletedAt
    );
}

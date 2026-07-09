using TransportSystem.Core.Application.Dtos.Fuel;
using TransportSystem.Core.Application.Dtos.Maintenance;

namespace TransportSystem.Core.Application.Dtos.Vehicle
{
    public record VehicleDetailDto(
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
        IReadOnlyList<MaintenanceRecordDto> MaintenanceRecords,
        IReadOnlyList<FuelRecordDto> FuelRecords,
        DateTime CreatedAt,
        bool IsDeleted,
        DateTime? DeletedAt
    );
}

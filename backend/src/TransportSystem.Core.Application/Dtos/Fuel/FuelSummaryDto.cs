namespace TransportSystem.Core.Application.Dtos.Fuel
{
    public record FuelSummaryDto(
        int Year,
        int Month,
        decimal TotalGallons,
        decimal TotalCost,
        decimal AveragePricePerGallon,
        int VehiclesWithRecords,
        int TotalVehicles,
        FuelConsumptionByVehicleDto? TopConsumer,
        IReadOnlyList<FuelConsumptionByVehicleDto> ConsumptionByVehicle
    );

    public record FuelConsumptionByVehicleDto(
        Guid VehicleId,
        string VehicleLabel,
        string LicensePlate,
        decimal Gallons,
        decimal TotalCost
    );
}

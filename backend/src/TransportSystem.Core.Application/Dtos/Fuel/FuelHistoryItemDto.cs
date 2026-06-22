namespace TransportSystem.Core.Application.Dtos.Fuel
{
    public record FuelHistoryItemDto(
        Guid Id,
        Guid VehicleId,
        string VehiclePlate,
        string VehicleLabel,
        DateTime RecordDate,
        decimal Gallons,
        decimal PricePerGallon,
        decimal TotalCost,
        decimal MileageAtRefuel,
        string? Notes,
        decimal? KmDriven,
        decimal? EfficiencyKmPerGallon
    );
}

namespace TransportSystem.Core.Application.Dtos.Fuel
{
    public record FuelRecordDto(
        Guid Id,
        DateTime RecordDate,
        decimal Gallons,
        decimal PricePerGallon,
        decimal TotalCost,
        decimal MileageAtRefuel,
        string? Notes
    );
}

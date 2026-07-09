namespace TransportSystem.WebApi.Contracts.Vehicles
{
    public record UpdateFuelRequest(
        DateTime RecordDate,
        decimal Gallons,
        decimal PricePerGallon,
        decimal MileageAtRefuel,
        string? Notes
    );
}

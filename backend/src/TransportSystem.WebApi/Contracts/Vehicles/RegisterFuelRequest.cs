namespace TransportSystem.WebApi.Contracts.Vehicles
{
    public record RegisterFuelRequest(
        DateTime RecordDate,
        decimal Gallons,
        decimal PricePerGallon,
        decimal MileageAtRefuel,
        string? Notes
    );
}

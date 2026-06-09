namespace TransportSystem.Core.Application.Dtos.Vehicle
{
    public record AvailableVehicleDto(
        Guid Id,
        string LicensePlate,
        string Brand,
        string Model,
        int Year,
        string Color,
        string Type,
        int Capacity
    );
}

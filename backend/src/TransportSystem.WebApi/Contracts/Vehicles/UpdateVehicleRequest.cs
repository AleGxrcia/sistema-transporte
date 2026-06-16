using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.WebApi.Contracts.Vehicles
{
    public record UpdateVehicleRequest(
        string Brand,
        string Model,
        string Color,
        VehicleType Type
    );
}

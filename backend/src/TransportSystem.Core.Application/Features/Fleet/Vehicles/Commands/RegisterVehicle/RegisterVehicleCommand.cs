using MediatR;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterVehicle
{
    public record RegisterVehicleCommand(
        string LicensePlate,
        int Capacity,
        string Brand,
        string Model,
        int Year,
        string Color,
        VehicleType Type
    ) : IRequest<Guid>;
}

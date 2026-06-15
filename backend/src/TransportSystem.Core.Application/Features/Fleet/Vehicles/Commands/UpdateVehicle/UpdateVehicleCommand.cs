using MediatR;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.UpdateVehicle
{
    public record UpdateVehicleCommand(
        Guid Id,
        string Brand,
        string Model,
        string Color,
        VehicleType Type
    ) : IRequest;
}

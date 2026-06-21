using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ReactivateVehicle
{
    public record ReactivateVehicleCommand(Guid Id) : IRequest;
}

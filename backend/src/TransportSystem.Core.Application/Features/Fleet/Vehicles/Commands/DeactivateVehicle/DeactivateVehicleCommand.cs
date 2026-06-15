using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.DeactivateVehicle
{
    public record DeactivateVehicleCommand(Guid Id) : IRequest;

}

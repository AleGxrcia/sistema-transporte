using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.DeleteVehicle
{
    public record DeleteVehicleCommand(Guid Id) : IRequest;

}

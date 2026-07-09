using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RestoreVehicle
{
    public record RestoreVehicleCommand(Guid Id) : IRequest;
}

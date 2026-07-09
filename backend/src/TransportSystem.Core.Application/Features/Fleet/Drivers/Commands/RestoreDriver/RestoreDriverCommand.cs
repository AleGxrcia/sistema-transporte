using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RestoreDriver
{
    public record RestoreDriverCommand(Guid Id) : IRequest;
}

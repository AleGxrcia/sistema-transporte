using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.DeleteDriver
{
    public record DeleteDriverCommand(Guid Id) : IRequest;
}

using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.SuspendDriver
{
    public record SuspendDriverCommand(Guid Id, string Reason) : IRequest;
}

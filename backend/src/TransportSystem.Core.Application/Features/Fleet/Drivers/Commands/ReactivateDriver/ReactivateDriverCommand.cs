using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.ReactivateDriver
{
    public record ReactivateDriverCommand(Guid Id) : IRequest;
}

using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.UpdateDriver
{
    public record UpdateDriverCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Phone,
        string? Address,
        Guid? SupervisorId
    ) : IRequest;
}

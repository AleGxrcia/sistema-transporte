using MediatR;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.StartTrip
{
    public record StartTripCommand(Guid RequestId) : IRequest;
}

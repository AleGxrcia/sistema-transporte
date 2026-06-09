using MediatR;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CompleteTrip
{
    public record RejectRequestCommand(Guid RequestId, string Reason) : IRequest;
}

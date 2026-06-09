using MediatR;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CancelRequest
{
    public record CancelRequestCommand(Guid RequestId, string Reason) : IRequest;
}

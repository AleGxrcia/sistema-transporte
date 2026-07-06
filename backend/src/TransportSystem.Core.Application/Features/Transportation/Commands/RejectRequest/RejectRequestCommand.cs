using MediatR;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.RejectRequest
{
    public record RejectRequestCommand(Guid RequestId, string Reason) : IRequest;
}

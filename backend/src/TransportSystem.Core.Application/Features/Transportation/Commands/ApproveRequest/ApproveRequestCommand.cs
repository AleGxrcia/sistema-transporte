using MediatR;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.ApproveRequest
{
    public record ApproveRequestCommand(Guid RequestId) : IRequest;
}

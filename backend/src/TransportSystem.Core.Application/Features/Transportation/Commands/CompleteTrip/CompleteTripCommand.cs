using MediatR;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CompleteTrip
{
    public record CompleteTripCommand(
        Guid RequestId,
        DateTime ActualDepartureTime,
        DateTime ActualReturnTime
    ) : IRequest;
}

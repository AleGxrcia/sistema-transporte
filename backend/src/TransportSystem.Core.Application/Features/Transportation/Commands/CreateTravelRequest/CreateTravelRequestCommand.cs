using MediatR;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CreateTravelRequest
{
    public record CreateTravelRequestCommand(
        string RequestingArea,
        int PassengerCount,
        string Destination,
        DateTime DepartureDateTime,
        DateTime ReturnDateTime,
        string TripPurpose
    ) : IRequest<Guid>;
}

namespace TransportSystem.Core.Application.Dtos.TravelRequest
{
    public record TravelRequestDto(
        Guid Id,
        string RequestNumber,
        string RequestingArea,
        int PassengerCount,
        string Destination,
        DateTime DepartureDateTime,
        DateTime ReturnDateTime,
        string TripPurpose,
        string Status,
        DateTime CreatedAt
    );
}

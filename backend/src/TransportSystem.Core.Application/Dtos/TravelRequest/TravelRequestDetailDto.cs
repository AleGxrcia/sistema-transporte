namespace TransportSystem.Core.Application.Dtos.TravelRequest
{
    public record TravelRequestDetailDto(
        Guid Id,
        string RequestNumber,
        string RequestingArea,
        int PassengerCount,
        string Destination,
        DateTime DepartureDateTime,
        DateTime ReturnDateTime,
        string TripPurpose,
        string Status,
        Guid RequestedByUserId,
        Guid? ApprovedByUserId,
        DateTime? ApprovedAt,
        string? RejectionReason,
        string? CancellationReason,
        Guid? CancelledByUserId,
        DateTime? CancelledAt,
        Guid? AssignedVehicleId,
        Guid? AssignedDriverId,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}

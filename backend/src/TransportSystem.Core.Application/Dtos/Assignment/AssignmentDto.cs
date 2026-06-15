namespace TransportSystem.Core.Application.Dtos.Assignment
{
    public record AssignmentDto(
        Guid AssignmentId,
        Guid RequestId,
        string RequestNumber,
        string VehiclePlate,
        string VehicleDescription,
        string DriverFullName,
        DateTime DepartureTime,
        DateTime ReturnTime,
        string Destination,
        string Status,
        string? CancellationReason,
        DateTime? ActualDepartureTime,
        DateTime? ActualReturnTime
    );
}

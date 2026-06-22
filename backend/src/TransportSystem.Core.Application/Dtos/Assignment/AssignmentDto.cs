namespace TransportSystem.Core.Application.Dtos.Assignment
{
    public record AssignmentDto(
        Guid AssignmentId,
        Guid RequestId,
        string RequestNumber,
        Guid VehicleId,
        string VehiclePlate,
        string VehicleDescription,
        Guid DriverId,
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

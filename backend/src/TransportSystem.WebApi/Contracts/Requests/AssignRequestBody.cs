namespace TransportSystem.WebApi.Contracts.Requests
{
    public record AssignRequestBody(Guid VehicleId, Guid DriverId);
}

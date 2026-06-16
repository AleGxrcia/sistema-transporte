namespace TransportSystem.WebApi.Contracts.Drivers
{
    public record UpdateDriverRequest(
        string Phone,
        string? Address,
        Guid? SupervisorId
    );
}

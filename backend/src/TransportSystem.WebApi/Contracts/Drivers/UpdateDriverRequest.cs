namespace TransportSystem.WebApi.Contracts.Drivers
{
    public record UpdateDriverRequest(
        string FirstName,
        string LastName,
        string Phone,
        string? Address,
        Guid? SupervisorId
    );
}

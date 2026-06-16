namespace TransportSystem.WebApi.Contracts.Auth
{
    public record LoginRequest(
        string Email,
        string Password
    );
}

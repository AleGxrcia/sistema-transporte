namespace TransportSystem.WebApi.Contracts.Auth
{
    public record RefreshRequest(
        string AccessToken,
        string RefreshToken
    );
}

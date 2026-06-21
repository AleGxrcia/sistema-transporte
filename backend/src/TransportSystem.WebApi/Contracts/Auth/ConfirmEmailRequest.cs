namespace TransportSystem.WebApi.Contracts.Auth
{
    public record ConfirmEmailRequest(
        string UserId,
        string Token
    );
}

namespace TransportSystem.WebApi.Contracts.Auth
{
    public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword
    );
}

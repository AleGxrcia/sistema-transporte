namespace TransportSystem.Core.Application.Dtos.Auth
{
    public record UpdateUserRequest(
        string? FirstName,
        string? LastName,
        string? Email
    );
}

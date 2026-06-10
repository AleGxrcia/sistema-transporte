using TransportSystem.Core.Application.Common.Enums;

namespace TransportSystem.Core.Application.Dtos.Auth
{
    public record CreateUserRequest(
        string Email,
        string FirstName,
        string LastName,
        string Password,
        UserRole Role,
        string Origin
    );
}

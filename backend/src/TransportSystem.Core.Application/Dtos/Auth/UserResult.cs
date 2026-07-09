using TransportSystem.Core.Application.Common.Enums;

namespace TransportSystem.Core.Application.Dtos.Auth
{
    public record UserResult(
        string Id,
        string Email,
        string FirstName,
        string LastName,
        UserRole Role,
        bool IsActive,
        bool IsDeleted
    );
}

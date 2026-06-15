using TransportSystem.Core.Application.Common.Enums;

namespace TransportSystem.Core.Application.Dtos.Auth
{
    public record AuthResult(
        bool Succeeded,
        string? UserId,
        string? Email,
        UserRole? Role,
        string? JwtToken,
        string? RefreshToken,
        string? Error = null
    );
}

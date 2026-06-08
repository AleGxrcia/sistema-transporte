using TransportSystem.Core.Application.Common.Enums;

namespace TransportSystem.Core.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        Guid Id { get; }
        string Email { get; }
        UserRole Role { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(UserRole role) => Role == role;

    }
}

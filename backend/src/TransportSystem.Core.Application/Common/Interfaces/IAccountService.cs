using TransportSystem.Core.Application.Dtos.Auth;

namespace TransportSystem.Core.Application.Common.Interfaces
{
    public interface IAccountService
    {
        Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        Task<UserResult> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task<UserResult> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
        Task SetUserActiveStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

        Task<UserResult> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserResult>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<string> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);
        Task SendForgotPasswordEmailAsync(string email, string origin, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);
    }
}

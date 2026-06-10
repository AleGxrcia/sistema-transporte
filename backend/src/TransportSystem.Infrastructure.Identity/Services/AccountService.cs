using Microsoft.AspNetCore.Identity;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Application.Dtos.Auth;
using TransportSystem.Infrastructure.Identity.Contexts;
using TransportSystem.Infrastructure.Identity.Entities;

namespace TransportSystem.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IdentityContext _identityContext;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IdentityContext identityContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityContext = identityContext;
        }

        public Task ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserResult> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<UserResult>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserResult> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SendForgotPasswordEmailAsync(string email, string origin, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SetUserActiveStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserResult> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

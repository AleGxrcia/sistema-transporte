using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TransportSystem.Core.Application.Common.Dtos;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Application.Dtos.Auth;
using TransportSystem.Infrastructure.Identity.Contexts;
using TransportSystem.Infrastructure.Identity.Entities;
using TransportSystem.Infrastructure.Identity.Settings;

namespace TransportSystem.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly IdentityContext _identityContext;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IEmailService emailService, IOptions<JwtSettings> jwtSettings, IdentityContext identityContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _jwtSettings = jwtSettings;
            _identityContext = identityContext;
        }

        public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                throw new UnauthorizedException("Credenciales inválidas.");

            if (!user.IsActive)
                throw new UnauthorizedException("La cuenta está desactivada.");

            if (!user.EmailConfirmed)
                throw new UnauthorizedException("La cuenta no ha sido confirmada. Revisa tu correo.");

            var signIn = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!signIn.Succeeded)
                throw new UnauthorizedException("Credenciales inválidas.");

            var role = await GetRoleAsync(user);
            var jwt = GenerateJwt(user, role.ToString());
            var refreshToken = CreateRefreshToken();

            user.RefreshTokens.Add(refreshToken);

            var oldTokens = user.RefreshTokens.Where(t => !t.IsActive).ToList();
            foreach (var old in oldTokens) user.RefreshTokens.Remove(old);

            await _userManager.UpdateAsync(user);

            return new AuthResult(
                Succeeded: true,
                UserId: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Role: role,
                JwtToken: new JwtSecurityTokenHandler().WriteToken(jwt),
                RefreshToken: refreshToken.Token
            );
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var user = await _identityContext.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken), cancellationToken);

            if (user is null)
                throw new UnauthorizedException("Token inválido.");

            var token = user.RefreshTokens.Single(t => t.Token == refreshToken);

            if (!token.IsActive)
                throw new UnauthorizedException("Token expirado o revocado.");

            user.RefreshTokens.Remove(token);

            var newRefresh = CreateRefreshToken();
            user.RefreshTokens.Add(newRefresh);

            var role = await GetRoleAsync(user);
            var jwt = GenerateJwt(user, role.ToString());

            await _userManager.UpdateAsync(user);

            return new AuthResult(
                Succeeded: true,
                UserId: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Role: role,
                JwtToken: new JwtSecurityTokenHandler().WriteToken(jwt),
                RefreshToken: newRefresh.Token
            );
        }

        public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var user = await _identityContext.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken), cancellationToken);

            if (user is null) return;

            var token = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken);
            if (token is null || !token.IsActive) return;

            user.RefreshTokens.Remove(token);
            await _userManager.UpdateAsync(user);
        }

        public async Task<UserResult> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing is not null)
                throw new ConflictException($"El email '{request.Email}' ya está registrado.");

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new ConflictException(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, request.Role.ToString());

            return ToUserResult(user, request.Role);
        }

        public async Task<UserResult> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var user = await FindOrThrowAsync(userId);

            if (request.FirstName is not null) user.FirstName = request.FirstName;
            if (request.LastName is not null) user.LastName = request.LastName;
            if (request.Email is not null)
            {
                user.Email = request.Email;
                user.UserName = request.Email;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ConflictException(string.Join("; ", result.Errors.Select(e => e.Description)));

            return ToUserResult(user, await GetRoleAsync(user));
        }

        public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await FindOrThrowAsync(userId);
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task SetUserActiveStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
        {
            var user = await FindOrThrowAsync(userId);
            user.IsActive = isActive;
            await _userManager.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await FindOrThrowAsync(userId);
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
                throw new ConflictException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task<UserResult> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await FindOrThrowAsync(userId);
            return ToUserResult(user, await GetRoleAsync(user));
        }

        public async Task<IReadOnlyList<UserResult>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken);

            var results = new List<UserResult>();
            foreach (var user in users)
            {
                var role = await GetRoleAsync(user);
                results.Add(ToUserResult(user, role));
            }

            return results;
        }

        public async Task<string> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
        {
            var user = await FindOrThrowAsync(userId);
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                throw new InvalidOperationException("Error al confirmar la cuenta. El token puede haber expirado.");

            return "Cuenta confirmada correctamente.";
        }

        public async Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new NotFoundException("Usuario no encontrado.");

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                throw new ConflictException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task SendForgotPasswordEmailAsync(string email, string origin, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return;

            var uri = await BuildResetPasswordUriAsync(user, origin);
            await _emailService.SendAsync(new EmailRequest(
                To: email,
                Subject: "Restablecer contraseña — TransFleet",
                Body: $"Restablece tu contraseña en: {uri}"
            ), cancellationToken);
        }

        private JwtSecurityToken GenerateJwt(ApplicationUser user, string role)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("uid", user.Id),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.DurationInMinutes),
                signingCredentials: creds
            );
        }

        private static RefreshToken CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(bytes),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7)
            };
        }

        private async Task<UserRole> GetRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleStr = roles.FirstOrDefault()
                ?? throw new InvalidOperationException("El usuario no tiene rol asignado.");
            return Enum.Parse<UserRole>(roleStr);
        }

        private async Task<ApplicationUser> FindOrThrowAsync(string userId)
            => await _userManager.FindByIdAsync(userId)
               ?? throw new NotFoundException($"Usuario '{userId}' no encontrado.");

        private async Task<string> BuildResetPasswordUriAsync(ApplicationUser user, string origin)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var uri = new Uri($"{origin}/auth/reset-password");
            var result = QueryHelpers.AddQueryString(uri.ToString(), "email", user.Email!);
            return QueryHelpers.AddQueryString(result, "token", code);
        }

        private static UserResult ToUserResult(ApplicationUser user, UserRole role)
            => new(user.Id, user.Email!, user.FirstName, user.LastName, role, user.IsActive);
    }
}

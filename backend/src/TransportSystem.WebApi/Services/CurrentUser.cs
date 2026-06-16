using System.Security.Claims;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;

namespace TransportSystem.WebApi.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid Id => Guid.Parse(
            _httpContextAccessor.HttpContext?.User?.FindFirstValue("uid")
            ?? throw new UnauthorizedException("Usuario no autenticado."));

        public string Email => _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public UserRole Role => Enum.Parse<UserRole>(
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role) ?? "Operator");

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated == true;
    }
}

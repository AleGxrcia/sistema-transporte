using Microsoft.AspNetCore.Identity;

namespace TransportSystem.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Soft delete: el usuario se archiva conservando la trazabilidad de sus acciones
        // (solicitudes creadas, aprobadas, asignadas). Nunca se elimina físicamente.
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}

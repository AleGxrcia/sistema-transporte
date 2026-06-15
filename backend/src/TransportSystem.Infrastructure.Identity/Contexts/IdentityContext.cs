using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Infrastructure.Identity.Entities;

namespace TransportSystem.Infrastructure.Identity.Contexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Identity");

            #region Configuracion de Tablas de Identity
            builder.Entity<ApplicationUser>(entity => 
            {
                entity.ToTable(name: "Users");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable(name: "UserRoles");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable(name: "UserLogins");
            });
            #endregion

            #region Configuracion de RefreshToken
            builder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");

                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.Token)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(rt => rt.ReplacedByToken)
                    .HasMaxLength(500);

                entity.HasOne(rt => rt.ApplicationUser)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;

namespace TransportSystem.Infrastructure.Persistence.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Drivers");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.Address)
                .HasMaxLength(300);

            builder.Property(d => d.SupervisorId); // nullable Guid

            builder.Property(d => d.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("StatusId");

            builder.Property(d => d.CreatedAt).IsRequired();
            builder.Property(d => d.UpdatedAt);

            builder.Property(d => d.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(d => d.DeletedAt);
            builder.Property(d => d.DeletedByUserId);

            // Value Object: NationalId
            builder.OwnsOne(d => d.NationalId, ni =>
            {
                ni.Property(x => x.Value)
                    .HasColumnName("IdNumber")
                    .IsRequired()
                    .HasMaxLength(20);

                ni.HasIndex(x => x.Value)
                    .IsUnique()
                    .HasDatabaseName("UQ_Drivers_IdNumber");
            });

            // Value Object: DriverLicense
            builder.OwnsOne(d => d.License, lic =>
            {
                lic.Property(x => x.Number)
                    .HasColumnName("LicenseNumber")
                    .IsRequired()
                    .HasMaxLength(50);

                lic.Property(x => x.Category)
                    .HasColumnName("LicenseCategoryId")
                    .IsRequired()
                    .HasConversion<int>();

                lic.Property(x => x.ExpirationDate)
                    .HasColumnName("LicenseExpirationDate")
                    .IsRequired()
                    .HasColumnType("date");

                lic.HasIndex(x => x.Number)
                    .IsUnique()
                    .HasDatabaseName("UQ_Drivers_LicenseNumber");

                lic.HasIndex(x => x.ExpirationDate)
                    .HasDatabaseName("IX_Drivers_LicenseExpiry");
            });

            // Indices
            builder.HasIndex(d => d.Status)
                .HasDatabaseName("IX_Drivers_StatusId");

            builder.HasIndex(d => d.SupervisorId)
                .HasDatabaseName("IX_Drivers_SupervisorId");

            builder.HasIndex(d => d.IsDeleted)
                .HasDatabaseName("IX_Drivers_IsDeleted");
        }
    }
}

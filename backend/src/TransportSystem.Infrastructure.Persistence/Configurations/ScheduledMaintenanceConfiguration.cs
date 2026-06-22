using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Infrastructure.Persistence.Configurations
{
    public class ScheduledMaintenanceConfiguration : IEntityTypeConfiguration<ScheduledMaintenance>
    {
        public void Configure(EntityTypeBuilder<ScheduledMaintenance> builder)
        {
            builder.ToTable("ScheduledMaintenances");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedNever();

            builder.Property(s => s.VehicleId).IsRequired();

            builder.Property(s => s.Type)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("MaintenanceTypeId");

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.ScheduledDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(s => s.Workshop)
                .HasMaxLength(200);

            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("StatusId");

            builder.Property(s => s.CreatedByUserId).IsRequired();
            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.ResultingMaintenanceRecordId);
            builder.Property(s => s.CancellationReason).HasMaxLength(500);

            // Value Object: Mileage
            builder.OwnsOne(s => s.ScheduledKm, km =>
            {
                km.Property(x => x.Value)
                    .HasColumnName("ScheduledKm")
                    .HasColumnType("decimal(10,2)");
            });

            builder.Navigation(s => s.ScheduledKm)
                .IsRequired(false);

            // Índices
            builder.HasIndex(s => s.VehicleId)
                .HasDatabaseName("IX_ScheduledMaintenance_VehicleId");

            builder.HasIndex(s => s.ScheduledDate)
                .HasDatabaseName("IX_ScheduledMaintenance_ScheduledDate");

            builder.HasIndex(s => s.Status)
                .HasDatabaseName("IX_ScheduledMaintenance_StatusId");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Infrastructure.Persistence.Configurations
{
    public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
    {
        public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
        {
            builder.ToTable("MaintenanceRecords");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.VehicleId).IsRequired();

            builder.Property(m => m.Type)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("MaintenanceTypeId");

            builder.Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(m => m.EntryDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(m => m.EstimatedExitDate)
                .HasColumnType("date");

            builder.Property(m => m.ActualExitDate)
                .HasColumnType("date");

            builder.Property(m => m.Cost)
                .HasColumnType("decimal(10,2)");

            builder.Property(m => m.Workshop)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.NextMaintenanceDateScheduled)
                .HasColumnType("date");

            // Value Object: Mileage
            builder.OwnsOne(m => m.NextMaintenanceKmScheduled, km =>
            {
                km.Property(x => x.Value)
                    .HasColumnName("NextMaintenanceKmScheduled")
                    .HasColumnType("decimal(10,2)")
                    .IsRequired(false);
            });

            builder.Property(m => m.CreatedAt).IsRequired();

            // Índices
            builder.HasIndex(m => m.VehicleId)
                .HasDatabaseName("IX_Maintenance_VehicleId");

            builder.HasIndex(m => m.EntryDate)
                .HasDatabaseName("IX_Maintenance_EntryDate");

            builder.HasIndex(m => m.NextMaintenanceDateScheduled)
                .HasDatabaseName("IX_Maintenance_NextDate");
        }
    }
}

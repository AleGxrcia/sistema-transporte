using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Infrastructure.Persistence.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Brand)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Year)
                .IsRequired();

            builder.Property(v => v.Color)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Type)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("VehicleTypeId");

            builder.Property(v => v.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("StatusId");

            builder.Property(v => v.LastMaintenanceDate)
                .HasColumnType("date");

            builder.Property(v => v.CreatedAt).IsRequired();
            builder.Property(v => v.UpdatedAt).IsRequired();

            // LicensePlate
            builder.OwnsOne(v => v.LicensePlate, lp =>
            {
                lp.Property(x => x.Value)
                    .HasColumnName("LicensePlate")
                    .IsRequired()
                    .HasMaxLength(20);

                lp.HasIndex(x => x.Value)
                    .IsUnique()
                    .HasDatabaseName("UQ_Vehicles_LicensePlate");
            });

            // VehicleCapacity
            builder.OwnsOne(v => v.Capacity, cap =>
            {
                cap.Property(x => x.Passengers)
                    .HasColumnName("Capacity")
                    .IsRequired();
            });

            // Mileage
            builder.OwnsOne(v => v.CurrentMileage, m =>
            {
                m.Property(x => x.Value)
                    .HasColumnName("Mileage")
                    .HasColumnType("decimal(10,2)")
                    .IsRequired()
                    .HasDefaultValue(0m);
            });

            // MaintenanceRecord
            builder.HasMany(v => v.MaintenanceRecords)
                .WithOne()
                .HasForeignKey(m => m.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(v => v.MaintenanceRecords)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_maintenanceRecords");

            // FuelRecord
            builder.HasMany(v => v.FuelRecords)
                .WithOne()
                .HasForeignKey(f => f.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(v => v.FuelRecords)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_fuelRecords");

            // Indices
            builder.HasIndex(v => v.Status)
                .HasDatabaseName("IX_Vehicles_StatusId");
        }
    }
}

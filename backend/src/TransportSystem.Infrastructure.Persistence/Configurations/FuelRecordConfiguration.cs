using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Infrastructure.Persistence.Configurations
{
    public class FuelRecordConfiguration : IEntityTypeConfiguration<FuelRecord>
    {
        public void Configure(EntityTypeBuilder<FuelRecord> builder)
        {
            builder.ToTable("FuelRecords");
            builder.HasKey(f => f.Id);

            builder.Property(f => f.VehicleId).IsRequired();

            builder.Property(f => f.RecordDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(f => f.Gallons)
                .IsRequired()
                .HasColumnType("decimal(8,3)");

            builder.Property(f => f.PricePerGallon)
                .IsRequired()
                .HasColumnType("decimal(8,2)");

            builder.Property(f => f.TotalCost)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            // Value Object: Mileage
            builder.OwnsOne(f => f.MileageAtRefuel, m =>
            {
                m.Property(x => x.Value)
                    .HasColumnName("MileageAtRefuel")
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();
            });

            builder.Property(f => f.Notes).HasMaxLength(300);
            builder.Property(f => f.CreatedAt).IsRequired();

            // Índices
            builder.HasIndex(f => f.VehicleId)
                .HasDatabaseName("IX_FuelRecords_VehicleId");

            builder.HasIndex(f => f.RecordDate)
                .HasDatabaseName("IX_FuelRecords_Date");

            builder.HasIndex(f => new { f.VehicleId, f.RecordDate })
                .HasDatabaseName("IX_FuelRecords_Vehicle_Date");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Core.Domain.Transportation.Entities;

namespace TransportSystem.Infrastructure.Persistence.Configurations
{
    internal class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignments");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.ScheduleId).IsRequired();
            builder.Property(a => a.RequestId).IsRequired();
            builder.Property(a => a.VehicleId).IsRequired();
            builder.Property(a => a.DriverId).IsRequired();
            builder.Property(a => a.AssignedByUserId).IsRequired();
            builder.Property(a => a.Notes).HasMaxLength(500);

            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("StatusId");

            builder.Property(a => a.CancellationReason).HasMaxLength(500);
            builder.Property(a => a.ActualDepartureTime);
            builder.Property(a => a.ActualReturnTime);
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.UpdatedAt);

            // TimeSlot
            builder.OwnsOne(a => a.TimeSlot, ts =>
            {
                ts.Property(x => x.DepartureTime)
                    .HasColumnName("DepartureTime")
                    .IsRequired();

                ts.Property(x => x.ReturnTime)
                    .HasColumnName("ReturnTime")
                    .IsRequired();
            });

            builder.HasIndex(a => a.RequestId)
                .IsUnique()
                .HasDatabaseName("UQ_Assignments_RequestId");

            // Indices
            builder.HasIndex(a => a.ScheduleId)
                .HasDatabaseName("IX_Assignments_ScheduleId");

            builder.HasIndex(a => a.Status)
                .HasDatabaseName("IX_Assignments_StatusId");
        }
    }
}

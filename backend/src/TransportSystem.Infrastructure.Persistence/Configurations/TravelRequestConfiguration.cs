using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Core.Domain.Transportation.Aggregates;

namespace TransportSystem.Infrastructure.Persistence.Configurations
{
    public class TravelRequestConfiguration : IEntityTypeConfiguration<TravelRequest>
    {
        public void Configure(EntityTypeBuilder<TravelRequest> builder)
        {
            builder.ToTable("TravelRequests");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.RequestingArea)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(r => r.PassengerCount)
                .IsRequired();

            builder.Property(r => r.TripPurpose)
                .IsRequired().HasMaxLength(500);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("StatusId");

            builder.Property(r => r.RequestedByUserId).IsRequired();
            builder.Property(r => r.ApprovedByUserId);
            builder.Property(r => r.ApprovedAt);
            builder.Property(r => r.RejectionReason).HasMaxLength(500);
            builder.Property(r => r.CancellationReason).HasMaxLength(500);
            builder.Property(r => r.CancelledByUserId);
            builder.Property(r => r.CancelledAt);
            builder.Property(r => r.AssignedVehicleId);
            builder.Property(r => r.AssignedDriverId);
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.UpdatedAt).IsRequired();

            // RequestNumber
            builder.OwnsOne(r => r.RequestNumber, rn =>
            {
                rn.Property(x => x.Value)
                    .HasColumnName("RequestNumber")
                    .IsRequired()
                    .HasMaxLength(20);

                rn.HasIndex(x => x.Value)
                    .IsUnique()
                    .HasDatabaseName("UQ_TravelRequests_Number");
            });

            // Destination
            builder.OwnsOne(r => r.Destination, dest =>
            {
                dest.Property(x => x.Name)
                    .HasColumnName("Destination")
                    .IsRequired()
                    .HasMaxLength(300);
            });

            // TimeSlot
            builder.OwnsOne(r => r.RequestedTimeSlot, ts =>
            {
                ts.Property(x => x.DepartureTime)
                    .HasColumnName("DepartureDateTime")
                    .IsRequired();

                ts.Property(x => x.ReturnTime)
                    .HasColumnName("ReturnDateTime")
                    .IsRequired();
            });

            // Indices
            builder.HasIndex(r => r.Status)
                .HasDatabaseName("IX_TravelRequests_StatusId");

            builder.HasIndex(r => r.RequestedByUserId)
                .HasDatabaseName("IX_TravelRequests_RequestedBy");

            builder.HasIndex(r => r.ApprovedByUserId)
                .HasDatabaseName("IX_TravelRequests_ApprovedBy");
        }
    }
}

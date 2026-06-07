using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportSystem.Core.Domain.Transportation.Aggregates;

namespace TransportSystem.Infrastructure.Persistence.Configurations
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable("Schedules");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.PeriodDate)
                .IsRequired()
                .HasColumnType("date");

            builder.HasIndex(s => s.PeriodDate)
                .IsUnique()
                .HasDatabaseName("UQ_Schedules_PeriodDate");

            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.UpdatedAt).IsRequired();

            // Assignment
            builder.HasMany(s => s.Assignments)
                .WithOne()
                .HasForeignKey(a => a.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(s => s.Assignments)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_assignments");
        }
    }
}

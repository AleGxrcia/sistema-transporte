using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle
{
    public class ScheduledMaintenance : Entity<Guid>
    {
        public Guid VehicleId { get; private set; }
        public MaintenanceType Type { get; private set; }
        public string Description { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public string? Workshop { get; private set; }
        public Mileage? ScheduledKm { get; private set; }
        public ScheduledMaintenanceStatus Status { get; private set; }
        public Guid CreatedByUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid? ResultingMaintenanceRecordId { get; private set; }
        public string? CancellationReason { get; private set; }

        private ScheduledMaintenance() { }

        internal static ScheduledMaintenance Create(
            Guid vehicleId,
            MaintenanceType type,
            string description,
            DateTime scheduledDate,
            Guid createdByUserId,
            string? workshop,
            Mileage? scheduledKm)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description);

            if (scheduledDate.Date < DateTime.UtcNow.Date)
                throw new DomainException("SCHEDULED_MAINTENANCE_PAST_DATE",
                    "La fecha programada no puede ser en el pasado.");

            return new ScheduledMaintenance
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicleId,
                Type = type,
                Description = description.Trim(),
                ScheduledDate = scheduledDate.Date,
                Workshop = workshop?.Trim(),
                ScheduledKm = scheduledKm,
                Status = ScheduledMaintenanceStatus.Pending,
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public bool IsPending => Status == ScheduledMaintenanceStatus.Pending;

        public int DaysRemaining => (ScheduledDate.Date - DateTime.UtcNow.Date).Days;

        internal void MarkAsExecuted(Guid maintenanceRecordId)
        {
            if (Status != ScheduledMaintenanceStatus.Pending)
                throw new DomainException("SCHEDULED_MAINTENANCE_INVALID_TRANSITION",
                    "Solo una programación pendiente puede ejecutarse.");

            Status = ScheduledMaintenanceStatus.Completed;
            ResultingMaintenanceRecordId = maintenanceRecordId;
        }

        internal void Cancel(string reason)
        {
            if (Status != ScheduledMaintenanceStatus.Pending)
                throw new DomainException("SCHEDULED_MAINTENANCE_INVALID_TRANSITION",
                    "Solo una programación pendiente puede cancelarse.");

            ArgumentException.ThrowIfNullOrWhiteSpace(reason);

            Status = ScheduledMaintenanceStatus.Cancelled;
            CancellationReason = reason.Trim();
        }
    }
}

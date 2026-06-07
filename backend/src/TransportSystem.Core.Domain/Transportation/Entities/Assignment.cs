using TransportSystem.Core.Domain.Common;
using TransportSystem.Core.Domain.Transportation.Enums;
using TransportSystem.Core.Domain.Transportation.ValueObjects;

namespace TransportSystem.Core.Domain.Transportation.Entities
{
    public class Assignment : Entity<Guid>
    {
        public Guid ScheduleId { get; private set; }
        public Guid RequestId { get; private set; }
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }
        public TimeSlot TimeSlot { get; private set; } = null!;
        public AssignmentStatus Status { get; private set; }
        public Guid AssignedByUserId { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? ActualDepartureTime { get; private set; }
        public DateTime? ActualReturnTime { get; private set; }
        public string? CancellationReason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Assignment() { }

        internal static Assignment Create(Guid scheduleId, Guid requestId, Guid vehicleId, Guid driverId,
            TimeSlot timeSlot, Guid assignedByUserId, string? notes = null)
        {
            return new Assignment
            {
                Id = Guid.NewGuid(),
                ScheduleId = scheduleId,
                RequestId = requestId,
                VehicleId = vehicleId,
                DriverId = driverId,
                TimeSlot = timeSlot,
                Status = AssignmentStatus.Scheduled,
                AssignedByUserId = assignedByUserId,
                Notes = notes?.Trim(),
                CreatedAt = DateTime.UtcNow,
            };
        }
        internal void Start()
        {
            if (Status != AssignmentStatus.Scheduled)
                throw new DomainException("ASSIGNMENT_INVALID_TRANSITION",
                    "Solo una asignación programada puede iniciarse.");

            ActualDepartureTime = DateTime.UtcNow;
            Status = AssignmentStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }

        internal void Complete(DateTime actualDeparture, DateTime actualReturn)
        {
            if (Status != AssignmentStatus.InProgress)
                throw new DomainException("ASSIGNMENT_INVALID_TRANSITION",
                    "Solo una asignación en curso puede completarse.");

            if (actualReturn <= actualDeparture)
                throw new DomainException("ASSIGNMENT_INVALID_TIMES",
                    "El tiempo de regreso real debe ser posterior al de salida real.");

            ActualDepartureTime = actualDeparture;
            ActualReturnTime = actualReturn;
            Status = AssignmentStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        internal void Cancel(string reason)
        {
            if (Status == AssignmentStatus.Completed)
                throw new DomainException("ASSIGNMENT_ALREADY_COMPLETED",
                    "No se puede cancelar una asignación ya completada.");

            if (Status == AssignmentStatus.Cancelled)
                throw new DomainException("ASSIGNMENT_ALREADY_CANCELLED",
                    "La asignación ya fue cancelada.");

            ArgumentException.ThrowIfNullOrWhiteSpace(reason);
            Status = AssignmentStatus.Cancelled;
            CancellationReason = reason.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsActive()
        {
            return Status is AssignmentStatus.Scheduled or AssignmentStatus.InProgress;
        }
    }
}
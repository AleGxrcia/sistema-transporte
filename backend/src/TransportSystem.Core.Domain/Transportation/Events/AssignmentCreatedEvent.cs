using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Transportation.Events
{
    public sealed record AssignmentCreatedEvent(
        Guid EventId,
        Guid ScheduleId,
        Guid AssignmentId,
        Guid VehicleId,
        Guid DriverId,
        DateTime OccurredAt) : IDomainEvent
    {
        public AssignmentCreatedEvent(Guid scheduleId, Guid assignmentId, Guid vehicleId, Guid driverId)
            : this(Guid.NewGuid(), scheduleId, assignmentId, vehicleId, driverId, DateTime.UtcNow) { }
    }
}

using TransportSystem.Core.Domain.Common;
using TransportSystem.Core.Domain.Transportation.Entities;
using TransportSystem.Core.Domain.Transportation.Events;
using TransportSystem.Core.Domain.Transportation.ValueObjects;

namespace TransportSystem.Core.Domain.Transportation.Aggregates
{
    public class Schedule : AggregateRoot<Guid>
    {
        public DateTime PeriodDate { get; private set; }

        private readonly List<Assignment> _assignments = [];
        public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private Schedule() { }

        public static Schedule CreateForDay(DateTime date)
        {
            var now = DateTime.UtcNow;
            return new Schedule
            {
                Id = Guid.NewGuid(),
                PeriodDate = date.Date,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        public Assignment AddAssignment(Guid requestId, Guid vehicleId, Guid driverId, TimeSlot timeSlot, Guid assignedByUserId)
        {
            var vehicleConflict = _assignments.FirstOrDefault(a =>
                a.VehicleId == vehicleId &&
                a.IsActive() &&
                a.TimeSlot.OverlapsWith(timeSlot));

            if (vehicleConflict is not null)
                throw new DomainException("SCHEDULE_VEHICLE_CONFLICT",
                    $"El vehículo ya tiene una asignación activa que se superpone: {vehicleConflict.TimeSlot}.");

            var driverConflict = _assignments.FirstOrDefault(a =>
                a.DriverId == driverId &&
                a.IsActive() &&
                a.TimeSlot.OverlapsWith(timeSlot));

            if (driverConflict is not null)
                throw new DomainException("SCHEDULE_DRIVER_CONFLICT",
                    $"El conductor ya tiene una asignación activa que se superpone: {driverConflict.TimeSlot}.");

            if (_assignments.Any(a => a.RequestId == requestId && a.IsActive()))
                throw new DomainException("SCHEDULE_DUPLICATE_REQUEST",
                    "Esta solicitud ya tiene una asignación activa.");

            var assignment = Assignment.Create(Id, requestId, vehicleId, driverId, timeSlot, assignedByUserId);
            _assignments.Add(assignment);
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new AssignmentCreatedEvent(Id, assignment.Id, vehicleId, driverId));

            return assignment;
        }

        public void CancelAssignment(Guid assignmentId, string reason)
        {
            var assignment = _assignments.FirstOrDefault(a => a.Id == assignmentId)
                ?? throw new DomainException("ASSIGNMENT_NOT_FOUND",
                    $"La asignación {assignmentId} no existe en este día.");

            assignment.Cancel(reason);
            UpdatedAt = DateTime.UtcNow;
        }

        public void StartAssignment(Guid assignmentId)
        {
            var assignment = _assignments.FirstOrDefault(a => a.Id == assignmentId)
                ?? throw new DomainException("ASSIGNMENT_NOT_FOUND",
                    $"La asignación {assignmentId} no existe en este día.");

            assignment.Start();
            UpdatedAt = DateTime.UtcNow;
        }

        public void CompleteAssignment(Guid assignmentId, DateTime actualDepartureTime, DateTime actualReturnTime)
        {
            var assignment = _assignments.FirstOrDefault(a => a.Id == assignmentId)
                ?? throw new DomainException("ASSIGNMENT_NOT_FOUND",
                    $"La asignación {assignmentId} no existe en este día.");

            assignment.Complete(actualDepartureTime, actualReturnTime);
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsVehicleAvailable(Guid vehicleId, TimeSlot timeSlot)
            => !_assignments.Any(a =>
                a.VehicleId == vehicleId && a.IsActive() && a.TimeSlot.OverlapsWith(timeSlot));

        public bool IsDriverAvailable(Guid driverId, TimeSlot timeSlot)
            => !_assignments.Any(a =>
                a.DriverId == driverId && a.IsActive() && a.TimeSlot.OverlapsWith(timeSlot));
    }
}

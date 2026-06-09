using TransportSystem.Core.Domain.Common;
using TransportSystem.Core.Domain.Transportation.Enums;
using TransportSystem.Core.Domain.Transportation.Events;
using TransportSystem.Core.Domain.Transportation.ValueObjects;

namespace TransportSystem.Core.Domain.Transportation.Aggregates
{
    public class TravelRequest : AggregateRoot<Guid>
    {
        public RequestNumber RequestNumber { get; private set; }
        public string RequestingArea { get; private set; }
        public int PassengerCount { get; private set; }
        public Destination Destination { get; private set; }
        public TimeSlot RequestedTimeSlot { get; private set; }
        public string TripPurpose { get; private set; }
        public RequestStatus Status { get; private set; }

        public Guid RequestedByUserId { get; private set; }
        public Guid? ApprovedByUserId { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public string? RejectionReason { get; private set; }
        public Guid? CancelledByUserId { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public string? CancellationReason { get; private set; }

        public Guid? AssignedVehicleId { get; private set; }
        public Guid? AssignedDriverId { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private TravelRequest() { }

        public static TravelRequest Create(int requestSequence, string requestingArea, Guid requestedByUserId,
            Destination destination, int passengerCount, TimeSlot requestedTimeSlot, string tripPurpose)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(requestingArea);
            ArgumentException.ThrowIfNullOrWhiteSpace(tripPurpose);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentNullException.ThrowIfNull(requestedTimeSlot);

            if (passengerCount <= 0)
                throw new DomainException("REQUEST_INVALID_PASSENGERS", "El número de colaboradores debe ser al menos 1.");

            var now = DateTime.UtcNow;
            return new TravelRequest
            {
                Id = Guid.NewGuid(),
                RequestNumber = RequestNumber.FromGuid(now.Year, Guid.NewGuid()),
                RequestingArea = requestingArea.Trim(),
                RequestedByUserId = requestedByUserId,
                Destination = destination,
                PassengerCount = passengerCount,
                RequestedTimeSlot = requestedTimeSlot,
                TripPurpose = tripPurpose.Trim(),
                Status = RequestStatus.Pending,
                CreatedAt = now,
            };
        }

        public void Approve(Guid approvedByUserId)
        {
            if (Status != RequestStatus.Pending)
                throw new DomainException("REQUEST_INVALID_TRANSITION",
                    $"Solo una solicitud pendiente puede aprobarse. Estado actual: '{Status}'.");

            var now = DateTime.UtcNow;
            ApprovedByUserId = approvedByUserId;
            ApprovedAt = now;
            Status = RequestStatus.Approved;
            UpdatedAt = now;
            RaiseDomainEvent(new RequestApprovedEvent(Id, approvedByUserId.ToString()));
        }

        public void Reject(string reason, Guid rejectedByUserId)
        {
            if (Status != RequestStatus.Pending)
                throw new DomainException("REQUEST_INVALID_TRANSITION",
                    "Solo una solicitud pendiente puede rechazarse.");

            ArgumentException.ThrowIfNullOrWhiteSpace(reason);

            Status = RequestStatus.Rejected;
            RejectionReason = reason.Trim();
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new RequestRejectedEvent(Id, reason, rejectedByUserId.ToString()));
        }

        public void AssignResources(Guid vehicleId, Guid driverId)
        {
            if (Status != RequestStatus.Approved)
                throw new DomainException("REQUEST_INVALID_TRANSITION",
                    "Solo una solicitud aprobada puede recibir recursos asignados.");

            AssignedVehicleId = vehicleId;
            AssignedDriverId = driverId;
            Status = RequestStatus.Assigned;
            UpdatedAt = DateTime.UtcNow;
        }

        public void StartTrip()
        {
            if (Status != RequestStatus.Assigned)
                throw new DomainException("REQUEST_INVALID_TRANSITION", "El viaje no tiene recursos asignados aún.");

            Status = RequestStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }

        public void CompleteTrip()
        {
            if (Status != RequestStatus.InProgress)
                throw new DomainException("REQUEST_INVALID_TRANSITION", "El viaje no está en progreso.");

            Status = RequestStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel(string reason, Guid cancelledByUserId)
        {
            if (Status is RequestStatus.Completed or RequestStatus.Rejected or RequestStatus.InProgress)
                throw new DomainException("REQUEST_INVALID_TRANSITION",
                    $"No se puede cancelar una solicitud en estado '{Status}'.");

            ArgumentException.ThrowIfNullOrWhiteSpace(reason);

            var now = DateTime.UtcNow;
            Status = RequestStatus.Cancelled;
            CancellationReason = reason.Trim();
            CancelledByUserId = cancelledByUserId;
            CancelledAt = now;
            UpdatedAt = now;
        }
    }
}

using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Transportation.Events
{
    public record RequestRejectedEvent(
        Guid EventId,
        Guid RequestId,
        string Reason,
        string RejectedByUserId,
        DateTime OccurredAt) : IDomainEvent
    {
        public RequestRejectedEvent(Guid requestId, string reason, string rejectedByUserId)
            : this(Guid.NewGuid(), requestId, reason, rejectedByUserId, DateTime.UtcNow) { }
    }
}

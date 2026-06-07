using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Transportation.Events
{
    public record RequestApprovedEvent(
        Guid EventId,
        Guid RequestId,
        string ApprovedByUserId,
        DateTime OccurredAt) : IDomainEvent
    {
        public RequestApprovedEvent(Guid requestId, string approvedByUserId)
            : this(Guid.NewGuid(), requestId, approvedByUserId, DateTime.UtcNow) { }
    }
}

using MediatR;

namespace TransportSystem.Core.Domain.Common
{
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
    }
}

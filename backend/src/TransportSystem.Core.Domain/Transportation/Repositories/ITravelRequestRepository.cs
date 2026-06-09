using TransportSystem.Core.Domain.Transportation.Aggregates;

namespace TransportSystem.Core.Domain.Transportation.Repositories
{
    public interface ITravelRequestRepository
    {
        Task<TravelRequest?> GetByIdAsync(
            Guid id, 
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TravelRequest>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<int> GetNextSequenceAsync(
            int year, 
            CancellationToken cancellationToken = default);

        Task AddAsync(TravelRequest request, CancellationToken cancellationToken = default);
        void Update(TravelRequest request);
        void Delete(TravelRequest request);

    }
}

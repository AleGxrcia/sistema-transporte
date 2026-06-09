using Microsoft.EntityFrameworkCore;
using TransportSystem.Core.Domain.Transportation.Aggregates;
using TransportSystem.Core.Domain.Transportation.Repositories;
using TransportSystem.Infrastructure.Persistence.Contexts;

namespace TransportSystem.Infrastructure.Persistence.Repositories
{
    public class TravelRequestRepository : ITravelRequestRepository
    {
        private readonly ApplicationContext _dbcontext;

        public TravelRequestRepository(ApplicationContext context)
        {
            _dbcontext = context;
        }

        public async Task AddAsync(TravelRequest request, CancellationToken cancellationToken = default)
        {
            await _dbcontext.TravelRequests.AddAsync(request, cancellationToken);
        }

        public void Update(TravelRequest request)
        {
            _dbcontext.TravelRequests.Update(request);
        }

        public void Delete(TravelRequest vehicle)
        {
            _dbcontext.Remove(vehicle);
        }

        public async Task<IReadOnlyList<TravelRequest>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbcontext.TravelRequests
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<TravelRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbcontext.TravelRequests.
                FirstOrDefaultAsync(tr => tr.Id == id, cancellationToken);
        }

        public Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

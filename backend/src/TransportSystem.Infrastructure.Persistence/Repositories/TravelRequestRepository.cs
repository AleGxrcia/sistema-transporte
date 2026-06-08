using Microsoft.EntityFrameworkCore;
using TransportSystem.Core.Domain.Transportation.Aggregates;
using TransportSystem.Core.Domain.Transportation.Repositories;
using TransportSystem.Infrastructure.Persistence.Contexts;

namespace TransportSystem.Infrastructure.Persistence.Repositories
{
    public class TravelRequestRepository : ITravelRequestRepository
    {
        private readonly ApplicationContext _context;

        public TravelRequestRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TravelRequest request, CancellationToken cancellationToken = default)
        {
            await _context.TravelRequests.AddAsync(request, cancellationToken);
        }

        public Task UpdateAsync(TravelRequest request, CancellationToken cancellationToken = default)
        {
           _context.TravelRequests.Update(request);
            return Task.CompletedTask;
        }

        public void Delete(TravelRequest vehicle)
        {
            _context.Remove(vehicle);
        }

        public async Task<IReadOnlyList<TravelRequest>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TravelRequests
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<TravelRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.TravelRequests.
                FirstOrDefaultAsync(tr => tr.Id == id, cancellationToken);
        }

        public Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

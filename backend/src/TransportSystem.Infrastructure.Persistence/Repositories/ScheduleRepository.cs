using Microsoft.EntityFrameworkCore;
using TransportSystem.Core.Domain.Transportation.Aggregates;
using TransportSystem.Core.Domain.Transportation.Repositories;
using TransportSystem.Infrastructure.Persistence.Contexts;

namespace TransportSystem.Infrastructure.Persistence.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationContext _dbContext;

        public ScheduleRepository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Schedule schedule, CancellationToken cancellationToken = default)
        {
            await _dbContext.Schedules.AddAsync(schedule, cancellationToken);
        }

        public Task UpdateAsync(Schedule schedule, CancellationToken cancellationToken = default)
        {
            _dbContext.Schedules.Update(schedule);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Schedule?> GetByDateWithAssignmentsAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Schedules
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.PeriodDate == date.Date, cancellationToken);
        }

        public async Task<Schedule?> GetByIdWithAssignmentsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Schedules
                .Include(s => s.Assignments)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
    }
}

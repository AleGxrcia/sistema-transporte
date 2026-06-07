using TransportSystem.Core.Domain.Transportation.Aggregates;

namespace TransportSystem.Core.Domain.Transportation.Repositories
{
    public interface IScheduleRepository
    {
        Task<Schedule?> GetByDateWithAssignmentsAsync(
            DateTime date, 
            CancellationToken cancellationToken = default);

        Task<Schedule?> GetByIdWithAssignmentsAsync(
            Guid id, 
            CancellationToken cancellationToken = default);

        Task AddAsync(Schedule schedule, CancellationToken cancellationToken = default);
        Task UpdateAsync(Schedule schedule, CancellationToken cancellationToken = default); 
        Task DeleteAsync(Guid id);
    }
}

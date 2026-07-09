using Microsoft.EntityFrameworkCore;
using TransportSystem.Core.Domain.Transportation.Aggregates;
using TransportSystem.Core.Domain.Transportation.Enums;
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

        public void Update(Schedule schedule)
        {
            _dbContext.Schedules.Update(schedule);
        }

        public void Delete(Schedule schedule)
        {
            _dbContext.Remove(schedule);
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

        public Task<bool> HasActiveAssignmentForVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Schedules
                .SelectMany(s => s.Assignments)
                .AnyAsync(a => a.VehicleId == vehicleId
                    && (a.Status == AssignmentStatus.Scheduled || a.Status == AssignmentStatus.InProgress),
                    cancellationToken);
        }

        public Task<bool> HasActiveAssignmentForDriverAsync(Guid driverId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Schedules
                .SelectMany(s => s.Assignments)
                .AnyAsync(a => a.DriverId == driverId
                    && (a.Status == AssignmentStatus.Scheduled || a.Status == AssignmentStatus.InProgress),
                    cancellationToken);
        }

        public Task<bool> HasAnyAssignmentForVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Schedules
                .SelectMany(s => s.Assignments)
                .AnyAsync(a => a.VehicleId == vehicleId, cancellationToken);
        }

        public Task<bool> HasAnyAssignmentForDriverAsync(Guid driverId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Schedules
                .SelectMany(s => s.Assignments)
                .AnyAsync(a => a.DriverId == driverId, cancellationToken);
        }
    }
}

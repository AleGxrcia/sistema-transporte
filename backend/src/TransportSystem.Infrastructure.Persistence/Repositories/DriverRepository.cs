using Microsoft.EntityFrameworkCore;
using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Infrastructure.Persistence.Contexts;

namespace TransportSystem.Infrastructure.Persistence.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly ApplicationContext _dbcontext;

        public DriverRepository(ApplicationContext context)
        {
            _dbcontext = context;
        }

        public async Task AddAsync(Driver driver, CancellationToken cancellationToken = default)
        {
            await _dbcontext.Drivers.AddAsync(driver, cancellationToken);
        }

        public void Update(Driver vehicle)
        {
            _dbcontext.Drivers.Update(vehicle);
        }

        public void Delete(Driver vehicle)
        {
            _dbcontext.Remove(vehicle);
        }

        public async Task<IReadOnlyList<Driver>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Drivers
                .Where(d => !d.IsDeleted)
                .AsNoTracking()
                .OrderBy(d => d.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Driver>> GetArchivedAsync(CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Drivers
                .Where(d => d.IsDeleted)
                .AsNoTracking()
                .OrderByDescending(d => d.DeletedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Driver>> GetAvailableForAssignmentAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;

            return await _dbcontext.Drivers
                .Where(d => !d.IsDeleted
                        && d.Status == DriverStatus.Available
                        && d.License.ExpirationDate >= today)
                .AsNoTracking()
                .OrderBy(d => d.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<Driver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Drivers.
                FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<Driver?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Drivers.
                FirstOrDefaultAsync(d => d.License.Number == licenseNumber, cancellationToken);
        }

        public async Task<Driver?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Drivers.
                FirstOrDefaultAsync(d => d.NationalId.Value == nationalId, cancellationToken);
        }

        public async Task<IReadOnlyList<Driver>> GetWithExpiringLicenseAsync(
            int withinDays, CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var thresholdDate = today.AddDays(withinDays);

            return await _dbcontext.Drivers
                .Where(d => !d.IsDeleted
                        && d.Status != DriverStatus.Inactive
                        && d.License.ExpirationDate >= today
                        && d.License.ExpirationDate <= thresholdDate)
                .AsNoTracking()
                .OrderBy(d => d.License.ExpirationDate)
                .ToListAsync(cancellationToken);
        }
    }
}

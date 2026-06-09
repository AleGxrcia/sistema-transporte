using Microsoft.EntityFrameworkCore;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Infrastructure.Persistence.Contexts;

namespace TransportSystem.Infrastructure.Persistence.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationContext _dbcontext;

        public VehicleRepository(ApplicationContext context)
        {
            _dbcontext = context;
        }

        public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
        {
            await _dbcontext.Vehicles.AddAsync(vehicle, cancellationToken);
        }

        public void Update(Vehicle vehicle)
        {
            _dbcontext.Vehicles.Update(vehicle);
        }

        public void Delete(Vehicle vehicle)
        {
            _dbcontext.Remove(vehicle);
        }

        public async Task<IReadOnlyList<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Vehicles
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Vehicle>> GetAvailableForAssignmentAsync(
            int minPassengers, 
            CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Vehicles
                .Where(v => v.Status == VehicleStatus.Available
                        && v.Capacity.Passengers >= minPassengers)
                .AsNoTracking()
                .OrderBy(v => v.Capacity.Passengers)
                .ToListAsync(cancellationToken);
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Vehicles.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        }

        public async Task<Vehicle?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Vehicles
                .Include(v => v.MaintenanceRecords)
                .Include(v => v.FuelRecords)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        }

        public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
        {
            return await _dbcontext.Vehicles.
                FirstOrDefaultAsync(v => v.LicensePlate.Value.Equals(
                    licensePlate, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
        }

        public async Task<IReadOnlyList<Vehicle>> GetWithUpcomingMaintenanceAsync(
            int withinDays, 
            CancellationToken cancellationToken = default)
        {
            var thresholdDate = DateTime.UtcNow.Date.AddDays(withinDays);

            return await _dbcontext.Vehicles
                .Include(v => v.MaintenanceRecords)
                .Where(v => v.MaintenanceRecords.Any(m =>
                    m.NextMaintenanceDateScheduled.HasValue &&
                    m.ActualExitDate == null &&
                    m.NextMaintenanceDateScheduled.Value <= thresholdDate))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}

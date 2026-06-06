using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Core.Domain.Fleet.Repositories
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Vehicle?> GetByIdWithDetailsAsync(
            Guid id, 
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Vehicle>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<Vehicle?> GetByLicensePlateAsync(
            string licensePlate, 
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Vehicle>> GetAvailableForAssignmentAsync(
            int minPassengers,
            CancellationToken cancellationToken = default);

        // Para alertas de mantenimiento en dashboard
        Task<IReadOnlyList<Vehicle>> GetWithUpcomingMaintenanceAsync(
            int withinDays,
            CancellationToken cancellationToken = default);

        Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
        void Update(Vehicle vehicle);
        void Delete(Vehicle vehicle);
    }
}

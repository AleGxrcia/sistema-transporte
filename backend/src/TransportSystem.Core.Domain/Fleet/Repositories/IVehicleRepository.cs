using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Core.Domain.Fleet.Repositories
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Vehicle>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Vehicle>> GetByStatusAsync(
            VehicleStatus status,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Vehicle>> GetAvailableForAssignmentAsync(
            int minPassengers,
            CancellationToken cancellationToken = default);

        Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
        void Update(Vehicle vehicle);
        void Delete(Vehicle vehicle);
    }
}

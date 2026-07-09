using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;

namespace TransportSystem.Core.Domain.Fleet.Repositories
{
    public interface IDriverRepository
    {
        Task<Driver?> GetByIdAsync(Guid id, CancellationToken 
            cancellationToken = default);

        Task<Driver?> GetByNationalIdAsync(string nationalId, 
            CancellationToken cancellationToken = default);

        Task<Driver?> GetByLicenseNumberAsync(string licenseNumber, 
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Driver>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Archivados (soft-deleted). Solo para gestión del Administrador.
        Task<IReadOnlyList<Driver>> GetArchivedAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Driver>> GetAvailableForAssignmentAsync(CancellationToken
            cancellationToken = default);

        // Para alertas del dashboard
        Task<IReadOnlyList<Driver>> GetWithExpiringLicenseAsync(int withinDays, CancellationToken
            cancellationToken = default);

        Task AddAsync(Driver driver, CancellationToken cancellationToken = default);
        void Update(Driver driver);
        void Delete(Driver driver);
    }
}

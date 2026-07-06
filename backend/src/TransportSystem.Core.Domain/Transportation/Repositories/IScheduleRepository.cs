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

        // Asignaciones activas (programadas o en curso) — gobiernan desactivar/suspender.
        Task<bool> HasActiveAssignmentForVehicleAsync(
            Guid vehicleId,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveAssignmentForDriverAsync(
            Guid driverId,
            CancellationToken cancellationToken = default);

        // Cualquier asignación (incluye historial) — gobierna la eliminación.
        Task<bool> HasAnyAssignmentForVehicleAsync(
            Guid vehicleId,
            CancellationToken cancellationToken = default);

        Task<bool> HasAnyAssignmentForDriverAsync(
            Guid driverId,
            CancellationToken cancellationToken = default);

        Task AddAsync(Schedule schedule, CancellationToken cancellationToken = default);
        void Update(Schedule schedule); 
        void Delete(Schedule schedule);
    }
}

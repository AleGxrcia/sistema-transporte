using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ExecuteScheduledMaintenance
{
    public class ExecuteScheduledMaintenanceCommandHandler : IRequestHandler<ExecuteScheduledMaintenanceCommand, Guid>
    {
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public ExecuteScheduledMaintenanceCommandHandler(IVehicleRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(ExecuteScheduledMaintenanceCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin) && !_currentUser.IsInRole(UserRole.Supervisor))
                throw new ForbiddenException("ejecutar mantenimiento programado", "Supervisor");

            var vehicle = await _repository.GetByIdWithDetailsAsync(command.VehicleId, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.VehicleId);

            Mileage? nextKm = command.NextMaintenanceKmScheduled.HasValue
                ? Mileage.Create(command.NextMaintenanceKmScheduled.Value)
                : null;

            var record = vehicle.ExecuteScheduledMaintenance(
                command.ScheduledMaintenanceId, command.EntryDate, command.Workshop, _currentUser.Id,
                command.EstimatedExitDate, command.NextMaintenanceDateScheduled, nextKm);

            _repository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return record.Id;
        }
    }
}

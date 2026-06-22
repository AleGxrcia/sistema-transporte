using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ScheduleMaintenance
{
    public class ScheduleMaintenanceCommandHandler : IRequestHandler<ScheduleMaintenanceCommand, Guid>
    {
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public ScheduleMaintenanceCommandHandler(IVehicleRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(ScheduleMaintenanceCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin) && !_currentUser.IsInRole(UserRole.Supervisor))
                throw new ForbiddenException("programar mantenimiento", "Supervisor");

            var vehicle = await _repository.GetByIdWithDetailsAsync(command.VehicleId, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.VehicleId);

            Mileage? scheduledKm = command.ScheduledKm.HasValue
                ? Mileage.Create(command.ScheduledKm.Value)
                : null;

            var scheduled = vehicle.ScheduleMaintenance(
                command.Type, command.Description, command.ScheduledDate,
                _currentUser.Id, command.Workshop, scheduledKm);

            _repository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return scheduled.Id;
        }
    }
}

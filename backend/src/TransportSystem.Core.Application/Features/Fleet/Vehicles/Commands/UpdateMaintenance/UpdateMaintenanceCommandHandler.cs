using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.UpdateMaintenance
{
    public class UpdateMaintenanceCommandHandler : IRequestHandler<UpdateMaintenanceCommand>
    {
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public UpdateMaintenanceCommandHandler(IVehicleRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(UpdateMaintenanceCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInAnyRole(UserRole.Admin, UserRole.Supervisor))
                throw new ForbiddenException("editar mantenimientos", "Administrador o Supervisor");

            var vehicle = await _repository.GetByIdWithDetailsAsync(command.VehicleId, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.VehicleId);

            Mileage? nextKm = command.NextMaintenanceKmScheduled.HasValue
                ? Mileage.Create(command.NextMaintenanceKmScheduled.Value)
                : null;

            vehicle.UpdateMaintenanceRecord(
                command.MaintenanceRecordId,
                command.Type,
                command.Description,
                command.EntryDate,
                command.Workshop,
                command.EstimatedExitDate,
                command.NextMaintenanceDateScheduled,
                nextKm);

            _repository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

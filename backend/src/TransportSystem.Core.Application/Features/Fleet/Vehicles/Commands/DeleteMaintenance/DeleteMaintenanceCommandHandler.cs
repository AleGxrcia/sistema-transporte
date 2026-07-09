using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.DeleteMaintenance
{
    public class DeleteMaintenanceCommandHandler : IRequestHandler<DeleteMaintenanceCommand>
    {
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public DeleteMaintenanceCommandHandler(IVehicleRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteMaintenanceCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInAnyRole(UserRole.Admin, UserRole.Supervisor))
                throw new ForbiddenException("eliminar mantenimientos", "Administrador o Supervisor");

            var vehicle = await _repository.GetByIdWithDetailsAsync(command.VehicleId, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.VehicleId);

            vehicle.DeleteMaintenanceRecord(command.MaintenanceRecordId);

            _repository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

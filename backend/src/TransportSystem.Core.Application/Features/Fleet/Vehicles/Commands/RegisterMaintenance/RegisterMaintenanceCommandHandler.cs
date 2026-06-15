using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterMaintenance
{
    public class RegisterMaintenanceCommandHandler : IRequestHandler<RegisterMaintenanceCommand, Guid>
    {
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public RegisterMaintenanceCommandHandler(IVehicleRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(RegisterMaintenanceCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin) && !_currentUser.IsInRole(UserRole.Supervisor))
                throw new ForbiddenException("registrar mantenimiento", "Supervisor");

            var vehicle = await _repository.GetByIdWithDetailsAsync(command.VehicleId, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.VehicleId);

            Mileage? nextKm = command.NextMaintenanceKmScheduled.HasValue
                ? Mileage.Create(command.NextMaintenanceKmScheduled.Value)
                : null;

            var record = vehicle.RegisterMaintenance(
                command.Type,
                command.Description,
                command.EntryDate,
                command.Workshop,
                _currentUser.Id,
                command.EstimatedExitDate,
                command.NextMaintenanceDateScheduled,
                nextKm);

            _repository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return record.Id;
        }
    }
}

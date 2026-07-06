using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Common;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.DeleteVehicle
{
    public class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand>
    {
        private readonly IVehicleRepository _repository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public DeleteVehicleCommandHandler(IVehicleRepository repository, IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteVehicleCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("eliminar vehículos", "Administrador");

            var vehicle = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.Id);

            if (await _scheduleRepository.HasAnyAssignmentForVehicleAsync(command.Id, cancellationToken))
                throw new DomainException("VEHICLE_HAS_TRIP_HISTORY",
                    "No se puede eliminar un vehículo con viajes registrados; desactívelo en su lugar.");

            _repository.Delete(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

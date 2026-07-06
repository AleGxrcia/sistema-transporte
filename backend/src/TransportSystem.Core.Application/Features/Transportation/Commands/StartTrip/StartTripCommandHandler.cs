using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.StartTrip
{
    public class StartTripCommandHandler : IRequestHandler<StartTripCommand>
    {
        private readonly ITravelRequestRepository _requestRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public StartTripCommandHandler(ITravelRequestRepository requestRepository, IScheduleRepository scheduleRepository,
            IVehicleRepository vehicleRepository, IDriverRepository driverRepository,
            IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _requestRepository = requestRepository;
            _scheduleRepository = scheduleRepository;
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(StartTripCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInAnyRole(UserRole.Admin, UserRole.Supervisor))
                throw new ForbiddenException("iniciar viajes", "Administrador o Supervisor");

            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken)
                ?? throw new NotFoundException("Solicitud", command.RequestId);

            var schedule = await _scheduleRepository.GetByDateWithAssignmentsAsync(
                request.RequestedTimeSlot.DepartureTime, cancellationToken)
                ?? throw new NotFoundException("Agenda del día", request.RequestedTimeSlot.DepartureTime.Date);

            var assignment = schedule.Assignments
                .FirstOrDefault(a => a.RequestId == request.Id)
                ?? throw new NotFoundException($"Asignación para la solicitud {command.RequestId}");

            var vehicle = await _vehicleRepository.GetByIdAsync(assignment.VehicleId, cancellationToken)
                ?? throw new NotFoundException("Vehículo", assignment.VehicleId);

            var driver = await _driverRepository.GetByIdAsync(assignment.DriverId, cancellationToken)
                ?? throw new NotFoundException("Conductor", assignment.DriverId);

            schedule.StartAssignment(assignment.Id);
            request.StartTrip();
            // El recurso pasa a "en viaje" recién cuando el viaje arranca.
            vehicle.MarkAsOnTrip();
            driver.MarkAsOnTrip();

            _scheduleRepository.Update(schedule);
            _requestRepository.Update(request);
            _vehicleRepository.Update(vehicle);
            _driverRepository.Update(driver);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

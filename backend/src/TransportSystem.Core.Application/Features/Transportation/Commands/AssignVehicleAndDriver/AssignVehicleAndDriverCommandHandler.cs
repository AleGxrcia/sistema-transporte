using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Common;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Aggregates;
using TransportSystem.Core.Domain.Transportation.Enums;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.AssignVehicleAndDriver
{
    public class AssignVehicleAndDriverCommandHandler : IRequestHandler<AssignVehicleAndDriverCommand, Guid>
    {
        private readonly ITravelRequestRepository _requestRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public AssignVehicleAndDriverCommandHandler(ITravelRequestRepository requestRepository, IScheduleRepository scheduleRepository,
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

        public async Task<Guid> Handle(AssignVehicleAndDriverCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Supervisor) && !_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("asignar vehículos y conductores", "Supervisor");

            // Cargar los 4 objetos
            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken)
                ?? throw new NotFoundException("Solicitud", command.RequestId);

            var vehicle = await _vehicleRepository.GetByIdAsync(command.VehicleId, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.VehicleId);

            var driver = await _driverRepository.GetByIdAsync(command.DriverId, cancellationToken)
                ?? throw new NotFoundException("Conductor", command.DriverId);

            var schedule = await _scheduleRepository.GetByDateWithAssignmentsAsync(
                request.RequestedTimeSlot.DepartureTime, cancellationToken);

            var isNewSchedule = schedule is null;
            if (isNewSchedule)
            {
                schedule = Schedule.CreateForDay(request.RequestedTimeSlot.DepartureTime);
                await _scheduleRepository.AddAsync(schedule, cancellationToken);
            }

            if (request.Status != RequestStatus.Approved)
                throw new DomainException("REQUEST_NOT_APPROVED",
                    $"La solicitud debe estar aprobada para asignar recursos. Estado actual: '{request.Status}'.");

            if (!vehicle.IsAvailableForAssignment())
                throw new DomainException("VEHICLE_UNAVAILABLE",
                    "El vehículo no está disponible para asignación.");

            if (driver.License.IsExpired())
                throw new DomainException("DRIVER_LICENSE_EXPIRED",
                    "El conductor no puede ser asignado porque su licencia está vencida.");

            if (!driver.IsAvailableForAssignment())
                throw new DomainException("DRIVER_UNAVAILABLE",
                    $"El conductor no está disponible para asignación. Estado actual: '{driver.Status}'.");

            if (!vehicle.Capacity.CanAccommodate(request.PassengerCount))
                throw new DomainException("VEHICLE_CAPACITY_INSUFFICIENT",
                    $"El vehículo tiene capacidad para {vehicle.Capacity.Passengers} pasajeros, " +
                    $"pero la solicitud requiere {request.PassengerCount}.");

            // Schedule valida disponibilidad de horario
            var assignment = schedule.AddAssignment(
                requestId: request.Id,
                vehicleId: vehicle.Id,
                driverId: driver.Id,
                timeSlot: request.RequestedTimeSlot,
                assignedByUserId: _currentUser.Id);

            request.AssignResources(vehicle.Id, driver.Id);
            vehicle.MarkAsOnTrip();
            driver.MarkAsOnTrip();

            _requestRepository.Update(request);
            _vehicleRepository.Update(vehicle);
            _driverRepository.Update(driver);
            if (!isNewSchedule)
                _scheduleRepository.Update(schedule);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}

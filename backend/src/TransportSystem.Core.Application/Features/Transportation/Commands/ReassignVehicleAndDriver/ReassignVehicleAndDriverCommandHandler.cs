using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Common;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Enums;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.ReassignVehicleAndDriver
{
    public class ReassignVehicleAndDriverCommandHandler : IRequestHandler<ReassignVehicleAndDriverCommand, Guid>
    {
        private readonly ITravelRequestRepository _requestRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public ReassignVehicleAndDriverCommandHandler(ITravelRequestRepository requestRepository, IScheduleRepository scheduleRepository,
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

        public async Task<Guid> Handle(ReassignVehicleAndDriverCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInAnyRole(UserRole.Admin, UserRole.Supervisor))
                throw new ForbiddenException("reasignar vehículos y conductores", "Administrador o Supervisor");

            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken)
                ?? throw new NotFoundException("Solicitud", command.RequestId);

            if (request.Status != RequestStatus.Assigned)
                throw new DomainException("REQUEST_NOT_ASSIGNED",
                    $"Solo una solicitud asignada (y no iniciada) puede reasignarse. Estado actual: '{request.Status}'.");

            var vehicle = await _vehicleRepository.GetByIdAsync(command.VehicleId, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.VehicleId);

            var driver = await _driverRepository.GetByIdAsync(command.DriverId, cancellationToken)
                ?? throw new NotFoundException("Conductor", command.DriverId);

            var schedule = await _scheduleRepository.GetByDateWithAssignmentsAsync(
                request.RequestedTimeSlot.DepartureTime, cancellationToken)
                ?? throw new DomainException("SCHEDULE_NOT_FOUND",
                    "No existe una agenda con la asignación de esta solicitud.");

            if (!vehicle.CanReceiveAssignment())
                throw new DomainException("VEHICLE_UNAVAILABLE",
                    $"El vehículo no está operativo para asignación. Estado actual: '{vehicle.Status}'.");

            if (driver.License.IsExpired())
                throw new DomainException("DRIVER_LICENSE_EXPIRED",
                    "El conductor no puede ser asignado porque su licencia está vencida.");

            if (!driver.CanReceiveAssignment())
                throw new DomainException("DRIVER_UNAVAILABLE",
                    $"El conductor no está operativo para asignación. Estado actual: '{driver.Status}'.");

            if (!vehicle.Capacity.CanAccommodate(request.PassengerCount))
                throw new DomainException("VEHICLE_CAPACITY_INSUFFICIENT",
                    $"El vehículo tiene capacidad para {vehicle.Capacity.Passengers} pasajeros, " +
                    $"pero la solicitud requiere {request.PassengerCount}.");

            // Schedule revalida que el nuevo vehículo/conductor no tengan solape de horario.
            schedule.ReassignAssignment(request.Id, vehicle.Id, driver.Id, _currentUser.Id);

            request.ReassignResources(vehicle.Id, driver.Id);

            _requestRepository.Update(request);
            _scheduleRepository.Update(schedule);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}

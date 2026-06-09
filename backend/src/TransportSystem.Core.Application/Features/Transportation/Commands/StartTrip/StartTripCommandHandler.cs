using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.StartTrip
{
    public class StartTripCommandHandler : IRequestHandler<StartTripCommand>
    {
        private readonly ITravelRequestRepository _requestRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public StartTripCommandHandler(ITravelRequestRepository requestRepository, IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _requestRepository = requestRepository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(StartTripCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Supervisor) && !_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("iniciar viajes", "Supervisor");

            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken)
                ?? throw new NotFoundException("Solicitud", command.RequestId);

            var schedule = await _scheduleRepository.GetByDateWithAssignmentsAsync(
                request.RequestedTimeSlot.DepartureTime, cancellationToken)
                ?? throw new NotFoundException("Agenda del día", request.RequestedTimeSlot.DepartureTime.Date);

            var assignment = schedule.Assignments
                .FirstOrDefault(a => a.RequestId == request.Id)
                ?? throw new NotFoundException($"Asignación para la solicitud {command.RequestId}");

            schedule.StartAssignment(assignment.Id);
            request.StartTrip();

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _requestRepository.UpdateAsync(request, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

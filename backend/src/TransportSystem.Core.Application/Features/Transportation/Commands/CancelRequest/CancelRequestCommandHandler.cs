using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Transportation.Enums;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CancelRequest
{
    public class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand>
    {
        private readonly ITravelRequestRepository _requestRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public CancelRequestCommandHandler(ITravelRequestRepository requestRepository,
            IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _requestRepository = requestRepository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(CancelRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken)
                ?? throw new NotFoundException("Solicitud", command.RequestId);

            var isOwner = request.RequestedByUserId == _currentUser.Id;
            var canManage = _currentUser.IsInAnyRole(UserRole.Admin, UserRole.Supervisor);

            if (!isOwner && !canManage)
                throw new ForbiddenException("cancelar esta solicitud", "Administrador, Supervisor o el solicitante original");

            // Si ya tenía recursos asignados, cancelar en cascada la asignación de la agenda
            // libera el horario del vehículo y del conductor (que siguen 'Available' hasta iniciar).
            var wasAssigned = request.Status == RequestStatus.Assigned;

            request.Cancel(command.Reason, _currentUser.Id);
            _requestRepository.Update(request);

            if (wasAssigned)
            {
                var schedule = await _scheduleRepository.GetByDateWithAssignmentsAsync(
                    request.RequestedTimeSlot.DepartureTime, cancellationToken);

                var assignment = schedule?.Assignments
                    .FirstOrDefault(a => a.RequestId == request.Id && a.IsActive());

                if (schedule is not null && assignment is not null)
                {
                    schedule.CancelAssignment(assignment.Id, command.Reason);
                    _scheduleRepository.Update(schedule);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

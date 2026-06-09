using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.ApproveRequest
{
    public class ApproveRequestCommandHandler : IRequestHandler<ApproveRequestCommand>
    {
        private readonly ITravelRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public ApproveRequestCommandHandler(ITravelRequestRepository requestRepository,
            IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(ApproveRequestCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Supervisor) && !_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("aprobar solicitudes", "Supervisor");

            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken)
                ?? throw new NotFoundException("Solicitud", command.RequestId);

            request.Approve(_currentUser.Id);

            _requestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

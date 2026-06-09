using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CancelRequest
{
    public class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand>
    {
        private readonly ITravelRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public CancelRequestCommandHandler(ITravelRequestRepository requestRepository,
            IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(CancelRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken)
                ?? throw new NotFoundException("Solicitud", command.RequestId);

            var isOwner = request.RequestedByUserId == _currentUser.Id;
            var isSupervisorOrAdmin =
                _currentUser.IsInRole(UserRole.Supervisor) || _currentUser.IsInRole(UserRole.Admin);

            if (!isOwner && !isSupervisorOrAdmin)
                throw new ForbiddenException("cancelar esta solicitud", "Supervisor o el solicitante original");

            request.Cancel(command.Reason, _currentUser.Id);

            _requestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

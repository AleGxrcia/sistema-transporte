using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.SuspendDriver
{
    public class SuspendDriverCommandHandler : IRequestHandler<SuspendDriverCommand>
    {
        private readonly IDriverRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public SuspendDriverCommandHandler(IDriverRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(SuspendDriverCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin) && !_currentUser.IsInRole(UserRole.Supervisor))
                throw new ForbiddenException("suspender conductores", "Supervisor");

            var driver = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new NotFoundException("Conductor", command.Id);

            driver.Suspend(command.Reason);

            _repository.Update(driver);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

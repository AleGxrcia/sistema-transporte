using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Common;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.SuspendDriver
{
    public class SuspendDriverCommandHandler : IRequestHandler<SuspendDriverCommand>
    {
        private readonly IDriverRepository _repository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public SuspendDriverCommandHandler(IDriverRepository repository, IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(SuspendDriverCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin)
                throw new ForbiddenException("suspender conductores", "Administrador");

            var driver = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new NotFoundException("Conductor", command.Id);

            if (await _scheduleRepository.HasActiveAssignmentForDriverAsync(command.Id, cancellationToken))
                throw new DomainException("DRIVER_HAS_ACTIVE_ASSIGNMENTS",
                    "No se puede suspender un conductor con viajes programados o en curso. Cancele o complete esos viajes primero.");

            driver.Suspend(command.Reason);

            _repository.Update(driver);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Common;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.DeleteDriver
{
    public class DeleteDriverCommandHandler : IRequestHandler<DeleteDriverCommand>
    {
        private readonly IDriverRepository _repository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public DeleteDriverCommandHandler(IDriverRepository repository, IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteDriverCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("eliminar conductores", "Administrador");

            var driver = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new NotFoundException("Conductor", command.Id);

            if (await _scheduleRepository.HasAnyAssignmentForDriverAsync(command.Id, cancellationToken))
                throw new DomainException("DRIVER_HAS_TRIP_HISTORY",
                    "No se puede eliminar un conductor con viajes registrados; desactívelo en su lugar.");

            _repository.Delete(driver);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

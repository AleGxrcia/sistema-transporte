using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.DeleteDriver
{
    public class DeleteDriverCommandHandler : IRequestHandler<DeleteDriverCommand>
    {
        private readonly IDriverRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public DeleteDriverCommandHandler(IDriverRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteDriverCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("eliminar conductores", "Administrador");

            var driver = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new NotFoundException("Conductor", command.Id);

            _repository.Delete(driver);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

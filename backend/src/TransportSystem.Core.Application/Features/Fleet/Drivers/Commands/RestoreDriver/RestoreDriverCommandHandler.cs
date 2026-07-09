using MediatR;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RestoreDriver
{
    public class RestoreDriverCommandHandler : IRequestHandler<RestoreDriverCommand>
    {
        private readonly IDriverRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public RestoreDriverCommandHandler(IDriverRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(RestoreDriverCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin)
                throw new ForbiddenException("restaurar conductores", "Administrador");

            var driver = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new NotFoundException("Conductor", command.Id);

            // El conductor restaurado vuelve a los listados operativos conservando su historial.
            driver.Restore();
            _repository.Update(driver);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

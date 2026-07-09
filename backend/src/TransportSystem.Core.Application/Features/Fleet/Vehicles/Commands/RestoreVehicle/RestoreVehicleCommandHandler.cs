using MediatR;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RestoreVehicle
{
    public class RestoreVehicleCommandHandler : IRequestHandler<RestoreVehicleCommand>
    {
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public RestoreVehicleCommandHandler(IVehicleRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(RestoreVehicleCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin)
                throw new ForbiddenException("restaurar vehículos", "Administrador");

            var vehicle = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.Id);

            // El vehículo restaurado vuelve a los listados operativos conservando su historial.
            vehicle.Restore();
            _repository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

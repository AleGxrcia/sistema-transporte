using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ReactivateVehicle
{
    public class ReactivateVehicleCommandHandler : IRequestHandler<ReactivateVehicleCommand>
    {
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public ReactivateVehicleCommandHandler(IVehicleRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(ReactivateVehicleCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("cambiar estado de vehículo", "Administrador");

            var vehicle = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.Id);

            vehicle.Reactivate();

            _repository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

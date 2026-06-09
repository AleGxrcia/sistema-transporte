using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterVehicle
{
    public class RegisterVehicleCommandHandler : IRequestHandler<RegisterVehicleCommand, Guid>
    {
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public RegisterVehicleCommandHandler(IVehicleRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(RegisterVehicleCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("registrar vehículos", "Administrador");

            var existing = await _repository.GetByLicensePlateAsync(command.LicensePlate, cancellationToken);
            if (existing is not null)
                throw new ConflictException("Vehículo", "matrícula", command.LicensePlate.ToUpperInvariant());

            var plate = LicensePlate.Create(command.LicensePlate);
            var capacity = VehicleCapacity.Create(command.Capacity);

            var vehicle = Vehicle.Register(command.Brand, command.Model, command.Year, 
                command.Color, command.Type, plate, capacity);

            await _repository.AddAsync(vehicle, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return vehicle.Id;
        }
    }
}

using MediatR;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterFuel
{
    public class RegisterFuelCommandHandler : IRequestHandler<RegisterFuelCommand, Guid>
    {
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public RegisterFuelCommandHandler(IVehicleRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(RegisterFuelCommand command, CancellationToken cancellationToken)
        {
            var vehicle = await _repository.GetByIdWithDetailsAsync(command.VehicleId, cancellationToken)
                ?? throw new NotFoundException("Vehículo", command.VehicleId);

            var mileage = Mileage.Create(command.MileageAtRefuel);

            var record = vehicle.RegisterFuel(
                command.RecordDate,
                command.Gallons,
                command.PricePerGallon,
                mileage,
                _currentUser.Id,
                command.Notes);

            _repository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return record.Id;
        }
    }
}

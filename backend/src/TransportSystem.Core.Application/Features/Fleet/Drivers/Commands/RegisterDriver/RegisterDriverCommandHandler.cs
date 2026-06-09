using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RegisterDriver
{
    public class RegisterDriverCommandHandler : IRequestHandler<RegisterDriverCommand, Guid>
    {
        private readonly IDriverRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public RegisterDriverCommandHandler(IDriverRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(RegisterDriverCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("registrar conductores", "Administrador");

            var existingByCedula = await _repository.GetByNationalIdAsync(command.NationalId, cancellationToken);
            if (existingByCedula is not null)
                throw new ConflictException("Conductor", "cédula", command.NationalId);

            var existingByLicense = await _repository.GetByLicenseNumberAsync(command.LicenseNumber, cancellationToken);
            if (existingByLicense is not null)
                throw new ConflictException("Conductor", "número de licencia", command.LicenseNumber);

            var nationalId = NationalId.Create(command.NationalId);
            var license = DriverLicense.Create(command.LicenseNumber, command.LicenseCategory, command.LicenseExpirationDate);

            var driver = Driver.Register(
                command.FirstName,
                command.LastName,
                nationalId,
                license,
                command.Phone,
                command.Address,
                command.SupervisorId
            );

            await _repository.AddAsync(driver, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return driver.Id;
        }
    }
}

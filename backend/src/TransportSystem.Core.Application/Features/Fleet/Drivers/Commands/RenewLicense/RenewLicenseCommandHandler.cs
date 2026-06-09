using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RenewLicense
{
    public class RenewLicenseCommandHandler : IRequestHandler<RenewLicenseCommand>
    {
        private readonly IDriverRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public RenewLicenseCommandHandler(IDriverRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(RenewLicenseCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(UserRole.Admin))
                throw new ForbiddenException("renovar licencias", "Administrador");

            var driver = await _repository.GetByIdAsync(command.DriverId, cancellationToken)
                ?? throw new NotFoundException("Conductor", command.DriverId);

            var newLicense = DriverLicense.Create(command.LicenseNumber, command.LicenseCategory, command.ExpirationDate);
            driver.RenewLicense(newLicense);

            _repository.Update(driver);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

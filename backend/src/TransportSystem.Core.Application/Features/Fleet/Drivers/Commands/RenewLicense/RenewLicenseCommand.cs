using MediatR;
using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RenewLicense
{
    public record RenewLicenseCommand(
        Guid DriverId,
        string LicenseNumber,
        LicenseCategory LicenseCategory,
        DateTime ExpirationDate
    ) : IRequest;
}

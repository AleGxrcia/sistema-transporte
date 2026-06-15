using MediatR;
using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RegisterDriver
{
    public record RegisterDriverCommand(
        string FirstName,
        string LastName,
        string NationalId,
        string LicenseNumber,
        LicenseCategory LicenseCategory,
        DateTime LicenseExpirationDate,
        string Phone,
        string? Address,
        Guid? SupervisorId
    ) : IRequest<Guid>;
}

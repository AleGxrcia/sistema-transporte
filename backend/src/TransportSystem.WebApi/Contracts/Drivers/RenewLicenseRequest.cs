using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;

namespace TransportSystem.WebApi.Contracts.Drivers
{
    public record RenewLicenseRequest(
        string LicenseNumber,
        LicenseCategory Category,
        DateTime ExpirationDate
    );
}

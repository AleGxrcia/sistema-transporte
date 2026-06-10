namespace TransportSystem.Core.Application.Dtos.Driver
{
    public record AvailableDriverDto(
        Guid Id,
        string firstName,
        string lastName,
        string LicenseNumber,
        string LicenseType,
        DateTime LicenseExpirationDate
    );
}

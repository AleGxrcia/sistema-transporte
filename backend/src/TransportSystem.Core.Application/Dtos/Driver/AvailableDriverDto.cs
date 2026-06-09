namespace TransportSystem.Core.Application.Dtos.Driver
{
    public record AvailableDriverDto(
        Guid Id,
        string FullName,
        string LicenseNumber,
        string LicenseType,
        DateTime LicenseExpirationDate
    );
}

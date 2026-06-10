namespace TransportSystem.Core.Application.Dtos.Driver
{
    public record DriverDto(
        Guid Id,
        string FirstName,
        string LastName,
        string NationalId,
        string LicenseNumber,
        string LicenseType,
        DateTime LicenseExpirationDate,
        bool LicenseExpired,
        bool LicenseExpiringSoon,
        string Phone,
        string? Address,
        string Status,
        Guid? SupervisorId,
        DateTime? CreatedAt,
        DateTime? UpdatedAt
    );
}

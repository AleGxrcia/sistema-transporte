using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Driver
{
    public class DriverLicense : ValueObject
    {
        public string Number { get; }
        public LicenseCategory Category { get; }
        public DateTime ExpirationDate { get; }

        public DriverLicense(string Number, LicenseCategory category, DateTime expirationDate)
        {
            Number = Number;
            Category = category;
            ExpirationDate = expirationDate;
        }

        public static DriverLicense Create(string number, LicenseCategory category, DateTime expirationDate)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new DomainException("LICENSE_NUMBER_EMPTY", "El número de licencia no puede estar vacío.");

            if (expirationDate.Date <= DateTime.UtcNow.Date)
                throw new DomainException("LICENSE_EXPIRED",
                    "No se puede registrar una licencia ya vencida. Ingrese una fecha válida.");

            return new DriverLicense(number.Trim().ToUpperInvariant(), category, expirationDate);
        }

        public bool IsExpired() => ExpirationDate < DateTime.UtcNow.Date;

        public bool ExpiresWithin(int days)
        {
           return !IsExpired() && ExpirationDate <= DateTime.UtcNow.Date.AddDays(days);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Number;
            yield return Category;
            yield return ExpirationDate;
        }
    }
}

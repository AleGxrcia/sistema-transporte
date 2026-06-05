using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle
{
    public class LicensePlate : ValueObject
    {
        public string Value { get; }

        private LicensePlate(string value)
        {
            Value = value;
        }

        public static LicensePlate Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("PLATE_EMPTY", "La placa no puede estar vacía.");

            var normalized = value.Trim().ToUpperInvariant().Replace("-", "").Replace(" ", "");

            if (normalized.Length < 4 || normalized.Length > 8)
                throw new DomainException("PLATE_INVALID_LENGTH", "La placa debe tener entre 4 y 8 caracteres alfanuméricos.");

            if (!normalized.All(char.IsLetterOrDigit))
                throw new DomainException("PLATE_INVALID_CHARS", "La placa solo puede contener letras y números.");

            return new LicensePlate(normalized);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}

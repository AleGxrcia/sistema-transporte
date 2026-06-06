using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Driver
{
    public class NationalId : ValueObject
    {
        public string Value { get; }

        private NationalId(string value)
        {
            Value = value;
        }

        public static NationalId Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("NATIONAL_ID_EMPTY", "El número de cédula no puede estar vacío.");

            var normalized = value.Trim().Replace("-", "").Replace(" ", "");

            if (!normalized.All(char.IsDigit))
                throw new DomainException("NATIONAL_ID_INVALID_CHARS", "La cédula solo puede contener dígitos.");

            if (normalized.Length != 11)
                throw new DomainException("NATIONAL_ID_INVALID_LENGTH", "La cédula debe tener 11 dígitos.");

            return new NationalId(normalized);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}

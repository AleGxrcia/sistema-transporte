using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Transportation.ValueObjects
{
    public class RequestNumber : ValueObject
    {
        public string Value { get; }

        private RequestNumber(string value)
        {
            Value = value;
        }

        public static RequestNumber Create(int year, int sequence)
        {
            if (year < 2026 || year > 2100)
                throw new DomainException("REQUEST_NUMBER_INVALID_YEAR",
                    $"El año {year} no es válido para un número de solicitud.");

            if (sequence <= 0)
                throw new DomainException("REQUEST_NUMBER_INVALID_SEQUENCE",
                    "El secuencial debe ser mayor a cero.");

            return new RequestNumber($"TR-{year}-{sequence:D4}");
        } 

        public static RequestNumber FromGuid(int year, Guid id)
        {
            if (year < 2026 || year > 2100)
                throw new DomainException("REQUEST_NUMBER_INVALID_YEAR",
                    $"El año {year} no es válido para un número de solicitud.");

            return new RequestNumber($"TR-{year}-{id.ToString("N")[..6].ToUpperInvariant()}");
        }

        public static RequestNumber Restore(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("REQUEST_NUMBER_EMPTY", "El número de solicitud no puede estar vacío.");

            return new RequestNumber(value.Trim().ToUpperInvariant());
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}

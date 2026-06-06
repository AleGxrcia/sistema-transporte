using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle
{
    public class Mileage : ValueObject
    {
        public decimal Value { get; }

        private Mileage(decimal value) => Value = value;

        public static Mileage Create(decimal value)
        {
            if (value < 0)
                throw new DomainException("MILEAGE_NEGATIVE", "El kilometraje no puede ser negativo.");

            return new Mileage(Math.Round(value, 2));
        }

        public bool IsGreaterThan(Mileage other) => Value > other.Value;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

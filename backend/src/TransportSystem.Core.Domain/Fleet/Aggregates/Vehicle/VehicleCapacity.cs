using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle
{
    public class VehicleCapacity : ValueObject
    {
        public int Passengers { get; }

        public VehicleCapacity(int passengers)
        {
            Passengers = passengers;
        }

        public static VehicleCapacity Create(int passengers)
        {
            if (passengers <= 0 || passengers > 100)
                throw new DomainException("CAPACITY_INVALID", "La capacidad debe ser entre 1 y 100 pasajeros.");

            return new VehicleCapacity(passengers);
        }

        public bool CanAccommodate(int requiredPassengers) => Passengers >= requiredPassengers;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Passengers;
        }
    }
}

using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Transportation.ValueObjects
{
    public class TimeSlot : ValueObject
    {
        public DateTime DepartureTime { get; private set; }
        public DateTime ReturnTime { get; private set; }

        private TimeSlot() { }

        private TimeSlot(DateTime departureTime, DateTime returnTime)
        {
            DepartureTime = departureTime;
            ReturnTime = returnTime;
        }

        public static TimeSlot Create(DateTime departure, DateTime returnTime)
        {
            if (departure >= returnTime)
                throw new DomainException("TIMESLOT_INVALID", 
                    "La hora de salida debe ser anterior a la hora de regreso.");

            if (departure < DateTime.UtcNow.AddMinutes(-5))
                throw new DomainException("TIMESLOT_IN_PAST", "No se puede crear un viaje en el pasado.");

            return new TimeSlot(departure, returnTime);
        }

        public bool OverlapsWith(TimeSlot other)
        {
            return DepartureTime < other.ReturnTime && other.DepartureTime < ReturnTime;
        }

        public TimeSpan Duration => ReturnTime - DepartureTime;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return DepartureTime;
            yield return ReturnTime;
        }
    }
}

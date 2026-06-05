using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle
{
    public class Vehicle : AggregateRoot<Guid>
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public LicensePlate LicensePlate { get; set; }
        public string VehicleType { get; set; }
        public VehicleCapacity Capacity { get; set; }
        public VehicleStatus Status { get; set; }
        public decimal CurrentKilometers { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        private Vehicle() { }

        public static Vehicle Register(string brand, string model, int year, LicensePlate licensePlate,
            string vehicleType, VehicleCapacity capacity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(brand, nameof(brand));
            ArgumentException.ThrowIfNullOrWhiteSpace(model, nameof(model));

            if (year < 1990 || year > DateTime.UtcNow.Year + 1)
                throw new DomainException("VEHICLE_INVALID_YEAR",
                    $"El año {year} no es válido para un vehículo de flota.");

            return new Vehicle
            {
                Id = Guid.NewGuid(),
                Brand = brand,
                Model = model,
                Year = year,
                LicensePlate = licensePlate,
                VehicleType = vehicleType,
                Capacity = capacity,
                Status = VehicleStatus.Available,
                CurrentKilometers = 0,
                RegisteredAt = DateTime.UtcNow,
            };
        }

        public void ReturnFromMaintenance()
        {
            if (Status != VehicleStatus.InMaintenance)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "Solo un vehículo en mantenimiento puede ser dado de alta.");

            Status = VehicleStatus.Available;
        }

        public void MarkAsOnTrip()
        {
            if (Status != VehicleStatus.Available)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "El vehículo no está disponible para asignarse a un viaje.");

            Status = VehicleStatus.OnTrip;
        }

        public void ReturnFromTrip()
        {
            if (Status != VehicleStatus.OnTrip)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "El vehículo no está registrado como en viaje.");

            Status = VehicleStatus.Available;
        }

        public void Deactivate()
        {
            if (Status == VehicleStatus.OnTrip)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "No se puede desactivar un vehículo que está actualmente en viaje.");

            Status = VehicleStatus.Inactive;
        }

        public bool IsAvailableForAssignment() => Status == VehicleStatus.Available;
    }
}

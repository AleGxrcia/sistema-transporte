using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle
{
    public class FuelRecord : Entity<Guid>
    {
        public Guid VehicleId { get; private set; }
        public DateTime RecordDate { get; private set; }
        public decimal Gallons { get; private set; }
        public decimal PricePerGallon { get; private set; }
        public decimal TotalCost { get; private set; }
        public Mileage MileageAtRefuel { get; private set; }
        public string? Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private FuelRecord() { }

        public static FuelRecord Create(Guid vehicleId, DateTime recordDate, decimal gallons, decimal pricePerGallon,
            Mileage mileageAtRefuel, string? notes)
        {
            if (gallons <= 0)
                throw new DomainException("FUEL_GALLONS_INVALID", "La cantidad de galones debe ser mayor a cero.");

            if (pricePerGallon <= 0)
                throw new DomainException("FUEL_PRICE_INVALID", "El precio por galón debe ser mayor a cero.");

            return new FuelRecord
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicleId,
                RecordDate = recordDate.Date,
                Gallons = Math.Round(gallons, 3),
                PricePerGallon = Math.Round(pricePerGallon, 2),
                TotalCost = Math.Round(gallons * pricePerGallon, 2),
                MileageAtRefuel = mileageAtRefuel,
                Notes = notes?.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}

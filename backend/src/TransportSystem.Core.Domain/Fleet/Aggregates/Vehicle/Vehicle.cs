using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle
{
    public class Vehicle : AggregateRoot<Guid>
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public VehicleType Type { get; set; }
        public LicensePlate LicensePlate { get; set; }
        public VehicleCapacity Capacity { get; set; }
        public Mileage CurrentMileage { get; private set; }
        public VehicleStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastMaintenanceDate { get; private set; }

        private readonly List<MaintenanceRecord> _maintenanceRecords = [];
        private readonly List<FuelRecord> _fuelRecords = [];

        public IReadOnlyCollection<MaintenanceRecord> MaintenanceRecords => _maintenanceRecords.AsReadOnly();
        public IReadOnlyCollection<FuelRecord> FuelRecords => _fuelRecords.AsReadOnly();

        private Vehicle() { }

        public static Vehicle Register(string brand, string model, int year, string color, VehicleType type,
            LicensePlate licensePlate, VehicleCapacity capacity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(brand, nameof(brand));
            ArgumentException.ThrowIfNullOrWhiteSpace(model, nameof(model));
            ArgumentException.ThrowIfNullOrWhiteSpace(color, nameof(color));


            if (year < 1990 || year > DateTime.UtcNow.Year + 1)
                throw new DomainException("VEHICLE_INVALID_YEAR",
                    $"El año {year} no es válido para un vehículo de flota.");

            return new Vehicle
            {
                Id = Guid.NewGuid(),
                Brand = brand.Trim(),
                Model = model.Trim(),
                Year = year,
                Color = color.Trim(),
                Type = type,
                LicensePlate = licensePlate,
                Capacity = capacity,
                CurrentMileage = Mileage.Create(0),
                Status = VehicleStatus.Available,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public void UpdateBasicInfo(string brand, string model, string color, VehicleType type)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(brand);
            ArgumentException.ThrowIfNullOrWhiteSpace(model);
            ArgumentException.ThrowIfNullOrWhiteSpace(color);

            Brand = brand.Trim();
            Model = model.Trim();
            Color = color.Trim();
            Type = type;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsOnTrip()
        {
            if (Status != VehicleStatus.Available)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    $"Solo un vehículo disponible puede asignarse a un viaje. Estado actual: '{Status}'.");

            Status = VehicleStatus.OnTrip;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReturnFromTrip()
        {
            if (Status != VehicleStatus.OnTrip)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "El vehículo no está registrado como en viaje.");

            Status = VehicleStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SendToMaintenance()
        {
            if (Status != VehicleStatus.Available)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    $"Solo un vehículo disponible puede enviarse a mantenimiento. Estado actual: '{Status}'.");

            Status = VehicleStatus.InMaintenance;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReturnFromMaintenance()
        {
            if (Status != VehicleStatus.InMaintenance)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "Solo un vehículo en mantenimiento puede ser dado de alta.");

            Status = VehicleStatus.Available;
        }

        public void ReturnFromMaintenance(DateTime maintenanceDate)
        {
            if (Status != VehicleStatus.InMaintenance)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "Solo un vehículo en mantenimiento puede ser dado de alta.");

            Status = VehicleStatus.Available;
            LastMaintenanceDate = maintenanceDate.Date;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (Status == VehicleStatus.OnTrip)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "No se puede desactivar un vehículo que está actualmente en viaje.");

            Status = VehicleStatus.Inactive;
        }

        public MaintenanceRecord RegisterMaintenance(MaintenanceType type, string description, DateTime entryDate, string workshop, 
            Guid registeredByUserId, DateTime? estimatedExitDate = null, DateTime? nextMaintenanceDateScheduled = null,
            Mileage? nextMaintenanceKmScheduled = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description);
            ArgumentException.ThrowIfNullOrWhiteSpace(workshop);

            SendToMaintenance();

            var record = MaintenanceRecord.Create(
                Id, type, description, entryDate, workshop,
                registeredByUserId, estimatedExitDate,
                nextMaintenanceDateScheduled, nextMaintenanceKmScheduled);

            _maintenanceRecords.Add(record);
            return record;
        }

        public void CloseMaintenanceRecord(Guid maintenanceRecordId, DateTime actualExitDate, decimal cost)
        {
            var record = _maintenanceRecords.FirstOrDefault(r => r.Id == maintenanceRecordId)
                ?? throw new DomainException("MAINTENANCE_NOT_FOUND",
                    $"El registro de mantenimiento {maintenanceRecordId} no existe para este vehículo.");

            record.Close(actualExitDate, cost);
            ReturnFromMaintenance(actualExitDate);
        }

        public FuelRecord RegisterFuel(DateTime recordDate, decimal gallons, decimal pricePerGallon, Mileage mileageAtRefuel,
            Guid registeredByUserId, string? notes = null)
        {
            if (!mileageAtRefuel.IsGreaterThan(CurrentMileage))
                throw new DomainException("FUEL_MILEAGE_INVALID",
                    $"El kilometraje al reabastecimiento ({mileageAtRefuel}) debe ser mayor " +
                    $"al kilometraje actual del vehículo ({CurrentMileage}).");

            var record = FuelRecord.Create(Id, recordDate, gallons, pricePerGallon,
                mileageAtRefuel, notes);

            _fuelRecords.Add(record);
            CurrentMileage = mileageAtRefuel;
            UpdatedAt = DateTime.UtcNow;

            return record;
        }

        public bool IsAvailableForAssignment() => Status == VehicleStatus.Available;

        // Alertas dashboard
        public bool HasUpcomingMaintenanceWithinDays(int days)
        {
            var upcoming = _maintenanceRecords
                .Where(r => r.NextMaintenanceDateScheduled.HasValue && !r.IsClosed)
                .Select(r => r.NextMaintenanceDateScheduled!.Value)
                .OrderBy(d => d)
                .FirstOrDefault();

            return upcoming != default && upcoming <= DateTime.UtcNow.Date.AddDays(days);
        }
    }
}

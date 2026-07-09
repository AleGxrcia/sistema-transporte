using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle
{
    public class Vehicle : AggregateRoot<Guid>, ISoftDeletable
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

        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public Guid? DeletedByUserId { get; private set; }

        private readonly List<MaintenanceRecord> _maintenanceRecords = [];
        private readonly List<FuelRecord> _fuelRecords = [];
        private readonly List<ScheduledMaintenance> _scheduledMaintenances = [];

        public IReadOnlyCollection<MaintenanceRecord> MaintenanceRecords => _maintenanceRecords.AsReadOnly();
        public IReadOnlyCollection<FuelRecord> FuelRecords => _fuelRecords.AsReadOnly();
        public IReadOnlyCollection<ScheduledMaintenance> ScheduledMaintenances => _scheduledMaintenances.AsReadOnly();

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

        public void Reactivate()
        {
            if (Status != VehicleStatus.Inactive)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "Solo un vehículo inactivo puede reactivarse.");

            Status = VehicleStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }

        // Soft delete: archiva el vehículo conservando su historial.
        public void MarkAsDeleted(Guid deletedByUserId)
        {
            if (Status == VehicleStatus.OnTrip)
                throw new DomainException("VEHICLE_INVALID_TRANSITION",
                    "No se puede archivar un vehículo que está actualmente en viaje.");

            if (IsDeleted) return;

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedByUserId = deletedByUserId;
            // Un vehículo archivado no puede quedar operativo: se marca inactivo
            // para que nunca aparezca como disponible mientras conserva su historial.
            Status = VehicleStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted) return;

            IsDeleted = false;
            DeletedAt = null;
            DeletedByUserId = null;
            // Al restaurar vuelve a estar disponible en los listados operativos.
            Status = VehicleStatus.Available;
            UpdatedAt = DateTime.UtcNow;
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

        public ScheduledMaintenance ScheduleMaintenance(MaintenanceType type, string description, DateTime scheduledDate,
            Guid createdByUserId, string? workshop = null, Mileage? scheduledKm = null)
        {
            var scheduled = ScheduledMaintenance.Create(
                Id, type, description, scheduledDate, createdByUserId, workshop, scheduledKm);

            _scheduledMaintenances.Add(scheduled);
            return scheduled;
        }

        public void CancelScheduledMaintenance(Guid scheduledMaintenanceId, string reason)
        {
            var scheduled = _scheduledMaintenances.FirstOrDefault(s => s.Id == scheduledMaintenanceId)
                ?? throw new DomainException("SCHEDULED_MAINTENANCE_NOT_FOUND",
                    $"La programación de mantenimiento {scheduledMaintenanceId} no existe para este vehículo.");

            scheduled.Cancel(reason);
        }

        public MaintenanceRecord ExecuteScheduledMaintenance(Guid scheduledMaintenanceId, DateTime entryDate, string workshop,
            Guid registeredByUserId, DateTime? estimatedExitDate = null, DateTime? nextMaintenanceDateScheduled = null,
            Mileage? nextMaintenanceKmScheduled = null)
        {
            var scheduled = _scheduledMaintenances.FirstOrDefault(s => s.Id == scheduledMaintenanceId)
                ?? throw new DomainException("SCHEDULED_MAINTENANCE_NOT_FOUND",
                    $"La programación de mantenimiento {scheduledMaintenanceId} no existe para este vehículo.");

            var record = RegisterMaintenance(scheduled.Type, scheduled.Description, entryDate, workshop,
                registeredByUserId, estimatedExitDate, nextMaintenanceDateScheduled, nextMaintenanceKmScheduled);

            scheduled.MarkAsExecuted(record.Id);

            return record;
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
            CurrentMileage = Mileage.Create(mileageAtRefuel.Value);
            UpdatedAt = DateTime.UtcNow;

            return record;
        }

        public void UpdateFuelRecord(Guid fuelRecordId, DateTime recordDate, decimal gallons, decimal pricePerGallon,
            Mileage mileageAtRefuel, string? notes)
        {
            var record = _fuelRecords.FirstOrDefault(r => r.Id == fuelRecordId)
                ?? throw new DomainException("FUEL_RECORD_NOT_FOUND",
                    $"El registro de combustible {fuelRecordId} no existe para este vehículo.");

            record.Update(recordDate, gallons, pricePerGallon, mileageAtRefuel, notes);
            RecalculateCurrentMileage();
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeleteFuelRecord(Guid fuelRecordId)
        {
            var record = _fuelRecords.FirstOrDefault(r => r.Id == fuelRecordId)
                ?? throw new DomainException("FUEL_RECORD_NOT_FOUND",
                    $"El registro de combustible {fuelRecordId} no existe para este vehículo.");

            _fuelRecords.Remove(record);
            RecalculateCurrentMileage();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMaintenanceRecord(Guid maintenanceRecordId, MaintenanceType type, string description,
            DateTime entryDate, string workshop, DateTime? estimatedExitDate,
            DateTime? nextMaintenanceDateScheduled, Mileage? nextMaintenanceKmScheduled)
        {
            var record = _maintenanceRecords.FirstOrDefault(r => r.Id == maintenanceRecordId)
                ?? throw new DomainException("MAINTENANCE_NOT_FOUND",
                    $"El registro de mantenimiento {maintenanceRecordId} no existe para este vehículo.");

            record.UpdateDetails(type, description, entryDate, workshop, estimatedExitDate,
                nextMaintenanceDateScheduled, nextMaintenanceKmScheduled);
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeleteMaintenanceRecord(Guid maintenanceRecordId)
        {
            var record = _maintenanceRecords.FirstOrDefault(r => r.Id == maintenanceRecordId)
                ?? throw new DomainException("MAINTENANCE_NOT_FOUND",
                    $"El registro de mantenimiento {maintenanceRecordId} no existe para este vehículo.");

            // Si se elimina un mantenimiento abierto que mantiene el vehículo en taller
            // (y no queda otro abierto), el vehículo vuelve a estar disponible.
            if (!record.IsClosed && Status == VehicleStatus.InMaintenance
                && !_maintenanceRecords.Any(r => r.Id != record.Id && !r.IsClosed))
            {
                Status = VehicleStatus.Available;
            }

            _maintenanceRecords.Remove(record);
            RecalculateLastMaintenanceDate();
            UpdatedAt = DateTime.UtcNow;
        }

        private void RecalculateCurrentMileage()
        {
            var maxMileage = _fuelRecords.Count > 0 ? _fuelRecords.Max(r => r.MileageAtRefuel.Value) : 0;
            CurrentMileage = Mileage.Create(maxMileage);
        }

        private void RecalculateLastMaintenanceDate()
        {
            var closed = _maintenanceRecords.Where(r => r.ActualExitDate.HasValue).ToList();
            LastMaintenanceDate = closed.Count > 0 ? closed.Max(r => r.ActualExitDate!.Value) : null;
        }

        public bool IsAvailableForAssignment() => Status == VehicleStatus.Available;

        public bool CanReceiveAssignment() =>
            Status != VehicleStatus.Inactive && Status != VehicleStatus.InMaintenance;

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

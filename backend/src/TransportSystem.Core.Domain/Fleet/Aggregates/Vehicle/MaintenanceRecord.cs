using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle
{
    public class MaintenanceRecord : Entity<Guid>
    {
        public Guid VehicleId { get; private set; }
        public MaintenanceType Type { get; private set; }
        public string Description { get; private set; }
        public DateTime EntryDate { get; private set; }
        public DateTime? EstimatedExitDate { get; private set; }
        public DateTime? ActualExitDate { get; private set; }
        public decimal? Cost { get; private set; }
        public string Workshop { get; private set; }
        public DateTime? NextMaintenanceDateScheduled { get; private set; }
        public Mileage? NextMaintenanceKmScheduled { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private MaintenanceRecord() { }

        internal static MaintenanceRecord Create(
            Guid vehicleId,
            MaintenanceType type,
            string description,
            DateTime entryDate,
            string workshop,
            Guid registeredByUserId,
            DateTime? estimatedExitDate,
            DateTime? nextMaintenanceDateScheduled,
            Mileage? nextMaintenanceKmScheduled)
        {
            return new MaintenanceRecord
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicleId,
                Type = type,
                Description = description.Trim(),
                EntryDate = entryDate.Date,
                Workshop = workshop.Trim(),
                EstimatedExitDate = estimatedExitDate?.Date,
                NextMaintenanceDateScheduled = nextMaintenanceDateScheduled?.Date,
                NextMaintenanceKmScheduled = nextMaintenanceKmScheduled,
                CreatedAt = DateTime.UtcNow
            };
        }

        public bool IsClosed => ActualExitDate.HasValue;

        internal void UpdateDetails(MaintenanceType type, string description, DateTime entryDate, string workshop,
            DateTime? estimatedExitDate, DateTime? nextMaintenanceDateScheduled, Mileage? nextMaintenanceKmScheduled)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description);
            ArgumentException.ThrowIfNullOrWhiteSpace(workshop);

            if (ActualExitDate.HasValue && ActualExitDate.Value < entryDate.Date)
                throw new DomainException("MAINTENANCE_INVALID_EXIT_DATE",
                    "La fecha de entrada no puede ser posterior a la fecha de salida real.");

            Type = type;
            Description = description.Trim();
            EntryDate = entryDate.Date;
            Workshop = workshop.Trim();
            EstimatedExitDate = estimatedExitDate?.Date;
            NextMaintenanceDateScheduled = nextMaintenanceDateScheduled?.Date;
            NextMaintenanceKmScheduled = nextMaintenanceKmScheduled;
        }

        internal void Close(DateTime actualExitDate, decimal cost)
        {
            if (IsClosed)
                throw new DomainException("MAINTENANCE_ALREADY_CLOSED", 
                    "Este registro de mantenimiento ya fue cerrado.");

            if (actualExitDate.Date < EntryDate)
                throw new DomainException("MAINTENANCE_INVALID_EXIT_DATE",
                    "La fecha de salida no puede ser anterior a la fecha de entrada.");

            if (cost < 0)
                throw new DomainException("MAINTENANCE_INVALID_COST", "El costo no puede ser negativo.");

            ActualExitDate = actualExitDate.Date;
            Cost = cost;
        }
    }
}

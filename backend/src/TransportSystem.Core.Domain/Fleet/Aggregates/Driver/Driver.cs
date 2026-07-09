using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Fleet.Aggregates.Driver
{
    public class Driver : AggregateRoot<Guid>, ISoftDeletable
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public NationalId NationalId { get; private set; }
        public string Phone { get; private set; }
        public string? Address { get; private set; }

        public DriverLicense License { get; private set; }
        public DriverStatus Status { get; private set; }
        public Guid? SupervisorId { get; private set; }

        public DateTime? CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public Guid? DeletedByUserId { get; private set; }

        private Driver() { }

        public static Driver Register(string firstName, string lastName, NationalId nationalId, DriverLicense license,
            string phone, string? address = null, Guid? supervisorId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
            ArgumentException.ThrowIfNullOrWhiteSpace(phone);
            ArgumentNullException.ThrowIfNull(nationalId);
            ArgumentNullException.ThrowIfNull(license);

            return new Driver
            {
                Id = Guid.NewGuid(),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                NationalId = nationalId,
                License = license,
                Phone = phone.Trim(),
                Address = address?.Trim(),
                SupervisorId = supervisorId,
                Status = DriverStatus.Available,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public bool IsAvailableForAssignment()
        {
            return Status == DriverStatus.Available && !License.IsExpired();
        }

        public bool CanReceiveAssignment()
        {
            return Status != DriverStatus.Inactive
                && Status != DriverStatus.Suspended
                && !License.IsExpired();
        }

        public void UpdatePersonalInfo(string firstName, string lastName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateContactInfo(string phone, string? address)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(phone);
            Phone = phone.Trim();
            Address = address?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignSupervisor(Guid? supervisorId)
        {
            SupervisorId = supervisorId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsOnTrip()
        {
            if (Status != DriverStatus.Available)
                throw new DomainException("DRIVER_INVALID_TRANSITION",
                    $"Solo un conductor disponible puede asignarse a un viaje. Estado: '{Status}'.");

            if (License.IsExpired())
                throw new DomainException("DRIVER_LICENSE_EXPIRED",
                    "No se puede asignar un conductor con licencia vencida.");

            Status = DriverStatus.OnTrip;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReturnFromTrip()
        {
            if (Status != DriverStatus.OnTrip)
                throw new DomainException("DRIVER_INVALID_TRANSITION",
                    "El conductor no está registrado como en viaje.");

            Status = DriverStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Suspend(string reason)
        {
            if (Status == DriverStatus.OnTrip)
                throw new DomainException("DRIVER_INVALID_TRANSITION",
                    "No se puede suspender un conductor que está en viaje.");

            if (Status == DriverStatus.Inactive)
                throw new DomainException("DRIVER_INVALID_TRANSITION",
                    "No se puede suspender un conductor inactivo.");

            ArgumentException.ThrowIfNullOrWhiteSpace(reason);
            Status = DriverStatus.Suspended;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            if (Status != DriverStatus.Suspended)
                throw new DomainException("DRIVER_INVALID_TRANSITION",
                    "Solo un conductor suspendido puede reactivarse.");

            if (License.IsExpired())
                throw new DomainException("DRIVER_LICENSE_EXPIRED",
                    "No se puede reactivar un conductor con licencia vencida. Renueve la licencia primero.");

            Status = DriverStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (Status == DriverStatus.OnTrip)
                throw new DomainException("DRIVER_INVALID_TRANSITION",
                    "No se puede desactivar un conductor que está en viaje.");

            Status = DriverStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RenewLicense(DriverLicense newLicense)
        {
            ArgumentNullException.ThrowIfNull(newLicense);
            License = newLicense;
            UpdatedAt = DateTime.UtcNow;
        }

        // Soft delete: archiva el conductor conservando su historial.
        public void MarkAsDeleted(Guid deletedByUserId)
        {
            if (Status == DriverStatus.OnTrip)
                throw new DomainException("DRIVER_INVALID_TRANSITION",
                    "No se puede archivar un conductor que está actualmente en viaje.");

            if (IsDeleted) return;

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedByUserId = deletedByUserId;
            // Un conductor archivado no puede quedar operativo: se marca inactivo
            // para que nunca aparezca como disponible mientras conserva su historial.
            Status = DriverStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted) return;

            IsDeleted = false;
            DeletedAt = null;
            DeletedByUserId = null;
            // Al restaurar vuelve a estar disponible en los listados operativos.
            Status = DriverStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

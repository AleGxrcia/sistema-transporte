using MediatR;
using TransportSystem.Core.Application.Dtos.Dashboard;
using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Enums;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetDashboardSummary
{
    public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
    {
        private const int MaintenanceAlertWindowDays = 7;
        private const int LicenseAlertWindowDays = 30;

        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ITravelRequestRepository _requestRepository;

        public GetDashboardSummaryQueryHandler(IVehicleRepository vehicleRepository, IDriverRepository driverRepository,
            ITravelRequestRepository requestRepository)
        {
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _requestRepository = requestRepository;
        }

        public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _vehicleRepository.GetAllAsync(cancellationToken);
            var drivers = await _driverRepository.GetAllAsync(cancellationToken);
            var requests = await _requestRepository.GetAllAsync(cancellationToken);

            var vehiclesWithUpcomingMaintenance = await _vehicleRepository.GetWithUpcomingMaintenanceAsync(
                MaintenanceAlertWindowDays, cancellationToken);
            var driversWithExpiringLicense = await _driverRepository.GetWithExpiringLicenseAsync(
                LicenseAlertWindowDays, cancellationToken);

            var today = DateTime.UtcNow.Date;

            var maintenanceAlerts = vehiclesWithUpcomingMaintenance.Select(v =>
            {
                var nextDate = v.MaintenanceRecords
                    .Where(m => m.NextMaintenanceDateScheduled.HasValue && !m.IsClosed)
                    .Min(m => m.NextMaintenanceDateScheduled!.Value);

                return $"{v.Brand} {v.Model} ({v.LicensePlate.Value}) - próximo mantenimiento: {nextDate:dd/MM/yyyy}";
            }).ToList();

            var licenseAlerts = driversWithExpiringLicense
                .Select(d => $"{d.FirstName} {d.LastName} - licencia vence: {d.License.ExpirationDate:dd/MM/yyyy}")
                .ToList();

            return new DashboardSummaryDto(
                TotalVehicles: vehicles.Count,
                AvailableVehicles: vehicles.Count(v => v.Status == VehicleStatus.Available),
                VehiclesOnTrip: vehicles.Count(v => v.Status == VehicleStatus.OnTrip),
                VehiclesInMaintenance: vehicles.Count(v => v.Status == VehicleStatus.InMaintenance),

                TotalDrivers: drivers.Count,
                AvailableDrivers: drivers.Count(d => d.Status == DriverStatus.Available),
                DriversOnTrip: drivers.Count(d => d.Status == DriverStatus.OnTrip),
                DriversWithExpiringLicense: driversWithExpiringLicense.Count,

                PendingRequests: requests.Count(r => r.Status == RequestStatus.Pending),
                ApprovedRequests: requests.Count(r => r.Status == RequestStatus.Approved),
                TodayTrips: requests.Count(r =>
                    r.RequestedTimeSlot.DepartureTime.Date == today &&
                    (r.Status == RequestStatus.Assigned || r.Status == RequestStatus.InProgress || r.Status == RequestStatus.Completed)),
                CompletedThisMonth: requests.Count(r =>
                    r.Status == RequestStatus.Completed &&
                    r.UpdatedAt.HasValue &&
                    r.UpdatedAt.Value.Year == today.Year &&
                    r.UpdatedAt.Value.Month == today.Month),

                MaintenanceAlerts: maintenanceAlerts,
                LicenseAlerts: licenseAlerts
            );
        }
    }
}

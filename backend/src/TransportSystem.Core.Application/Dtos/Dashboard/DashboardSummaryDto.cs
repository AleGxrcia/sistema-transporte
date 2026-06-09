namespace TransportSystem.Core.Application.Dtos.Dashboard
{
    public record DashboardSummaryDto(
        // Vehículos
        int TotalVehicles,
        int AvailableVehicles,
        int VehiclesOnTrip,
        int VehiclesInMaintenance,

        // Conductores
        int TotalDrivers,
        int AvailableDrivers,
        int DriversOnTrip,
        int DriversWithExpiringLicense,

        // Solicitudes
        int PendingRequests,
        int ApprovedRequests,
        int TodayTrips,
        int CompletedThisMonth,

        // Alertas
        IReadOnlyList<string> MaintenanceAlerts,
        IReadOnlyList<string> LicenseAlerts
    );

}

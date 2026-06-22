namespace TransportSystem.Core.Application.Dtos.Reports
{
    public record ReportsSummaryDto(
        int Year,
        int Month,

        // KPIs del mes seleccionado
        int TripsCompleted,
        double CompletionRatePercent,
        int CancelledCount,
        decimal FuelCost,
        decimal FuelGallons,
        decimal MaintenanceCost,
        int MaintenanceServicesCount,

        IReadOnlyList<MonthlyTripCountDto> TripsByMonth,
        IReadOnlyList<AreaRequestCountDto> RequestsByArea,
        IReadOnlyList<DriverTripsReportDto> TopDrivers,
        IReadOnlyList<VehicleUsageReportDto> TopVehicles,
        IReadOnlyList<MonthlyFuelDto> FuelByMonth
    );

    public record MonthlyTripCountDto(int Year, int Month, string MonthLabel, int Count);

    public record AreaRequestCountDto(string Area, int Count);

    public record DriverTripsReportDto(Guid DriverId, string DriverName, int TripsCompleted, int TripsCancelled);

    public record VehicleUsageReportDto(Guid VehicleId, string VehicleLabel, string LicensePlate, int TripsCompleted);

    public record MonthlyFuelDto(int Year, int Month, string MonthLabel, decimal Gallons, decimal Cost);
}

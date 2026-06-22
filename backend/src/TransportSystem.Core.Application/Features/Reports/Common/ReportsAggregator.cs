using TransportSystem.Core.Application.Dtos.Reports;
using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Transportation.Aggregates;
using TransportSystem.Core.Domain.Transportation.Enums;

namespace TransportSystem.Core.Application.Features.Reports.Common
{
    public static class ReportsAggregator
    {
        private static readonly string[] MonthLabels =
        [
            "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"
        ];

        private const int TrailingMonths = 6;

        public static ReportsSummaryDto BuildSummary(
            IReadOnlyList<Vehicle> vehicles,
            IReadOnlyList<Driver> drivers,
            IReadOnlyList<TravelRequest> requests,
            int year,
            int month)
        {
            var requestsInMonth = requests
                .Where(r => r.RequestedTimeSlot.DepartureTime.Year == year && r.RequestedTimeSlot.DepartureTime.Month == month)
                .ToList();

            var tripsCompleted = requestsInMonth.Count(r => r.Status == RequestStatus.Completed);
            var cancelledCount = requestsInMonth.Count(r => r.Status == RequestStatus.Cancelled);
            var completionRate = (tripsCompleted + cancelledCount) > 0
                ? Math.Round(tripsCompleted * 100.0 / (tripsCompleted + cancelledCount), 1)
                : 0;

            var maintenanceRecordsInMonth = vehicles
                .SelectMany(v => v.MaintenanceRecords)
                .Where(m => m.EntryDate.Year == year && m.EntryDate.Month == month)
                .ToList();

            var fuelRecordsInMonth = vehicles
                .SelectMany(v => v.FuelRecords)
                .Where(f => f.RecordDate.Year == year && f.RecordDate.Month == month)
                .ToList();

            var driverNames = drivers.ToDictionary(d => d.Id, d => $"{d.FirstName} {d.LastName}");
            var vehicleInfo = vehicles.ToDictionary(v => v.Id, v => ($"{v.Brand} {v.Model}", v.LicensePlate.Value));

            var topDrivers = requestsInMonth
                .Where(r => r.AssignedDriverId.HasValue)
                .GroupBy(r => r.AssignedDriverId!.Value)
                .Select(g => new DriverTripsReportDto(
                    g.Key,
                    driverNames.TryGetValue(g.Key, out var name) ? name : "Conductor desconocido",
                    g.Count(r => r.Status == RequestStatus.Completed),
                    g.Count(r => r.Status == RequestStatus.Cancelled)))
                .OrderByDescending(d => d.TripsCompleted)
                .Take(5)
                .ToList();

            var topVehicles = requestsInMonth
                .Where(r => r.AssignedVehicleId.HasValue && r.Status == RequestStatus.Completed)
                .GroupBy(r => r.AssignedVehicleId!.Value)
                .Select(g => new VehicleUsageReportDto(
                    g.Key,
                    vehicleInfo.TryGetValue(g.Key, out var info) ? info.Item1 : "Vehículo desconocido",
                    vehicleInfo.TryGetValue(g.Key, out var info2) ? info2.Item2 : "—",
                    g.Count()))
                .OrderByDescending(v => v.TripsCompleted)
                .Take(5)
                .ToList();

            var requestsByArea = requestsInMonth
                .GroupBy(r => r.RequestingArea)
                .Select(g => new AreaRequestCountDto(g.Key, g.Count()))
                .OrderByDescending(a => a.Count)
                .ToList();

            var months = BuildTrailingMonths(year, month);

            var tripsByMonth = months
                .Select(m => new MonthlyTripCountDto(
                    m.Year, m.Month, MonthLabels[m.Month - 1],
                    requests.Count(r => r.Status == RequestStatus.Completed
                        && r.RequestedTimeSlot.DepartureTime.Year == m.Year
                        && r.RequestedTimeSlot.DepartureTime.Month == m.Month)))
                .ToList();

            var fuelByMonth = months
                .Select(m =>
                {
                    var recordsInThatMonth = vehicles
                        .SelectMany(v => v.FuelRecords)
                        .Where(f => f.RecordDate.Year == m.Year && f.RecordDate.Month == m.Month)
                        .ToList();

                    return new MonthlyFuelDto(
                        m.Year, m.Month, MonthLabels[m.Month - 1],
                        recordsInThatMonth.Sum(f => f.Gallons),
                        recordsInThatMonth.Sum(f => f.TotalCost));
                })
                .ToList();

            return new ReportsSummaryDto(
                year, month,
                tripsCompleted,
                completionRate,
                cancelledCount,
                fuelRecordsInMonth.Sum(f => f.TotalCost),
                fuelRecordsInMonth.Sum(f => f.Gallons),
                maintenanceRecordsInMonth.Sum(m => m.Cost ?? 0),
                maintenanceRecordsInMonth.Count,
                tripsByMonth,
                requestsByArea,
                topDrivers,
                topVehicles,
                fuelByMonth
            );
        }

        private static List<(int Year, int Month)> BuildTrailingMonths(int year, int month)
        {
            var result = new List<(int Year, int Month)>();
            var cursor = new DateTime(year, month, 1);

            for (var i = TrailingMonths - 1; i >= 0; i--)
            {
                var date = cursor.AddMonths(-i);
                result.Add((date.Year, date.Month));
            }

            return result;
        }
    }
}

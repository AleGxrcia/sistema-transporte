using MediatR;
using TransportSystem.Core.Application.Dtos.Maintenance;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetMaintenanceAlerts
{
    public class GetMaintenanceAlertsQueryHandler
        : IRequestHandler<GetMaintenanceAlertsQuery, IReadOnlyList<MaintenanceAlertDto>>
    {
        private readonly IVehicleRepository _repository;

        public GetMaintenanceAlertsQueryHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<MaintenanceAlertDto>> Handle(
            GetMaintenanceAlertsQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetAllWithFullDetailsAsync(cancellationToken);
            var today = DateTime.UtcNow.Date;
            var threshold = today.AddDays(request.WithinDays);

            var scheduledAlerts = vehicles
                .SelectMany(v => v.ScheduledMaintenances
                    .Where(s => s.Status == ScheduledMaintenanceStatus.Pending && s.ScheduledDate <= threshold)
                    .Select(s => new MaintenanceAlertDto(
                        v.Id,
                        $"{v.Brand} {v.Model}",
                        v.LicensePlate.Value,
                        s.Type.ToString(),
                        s.ScheduledDate,
                        (s.ScheduledDate.Date - today).Days,
                        "Scheduled"
                    )));

            var recordAlerts = vehicles
                .SelectMany(v => v.MaintenanceRecords
                    .Where(m => m.NextMaintenanceDateScheduled.HasValue && !m.IsClosed
                        && m.NextMaintenanceDateScheduled.Value <= threshold)
                    .Select(m => new MaintenanceAlertDto(
                        v.Id,
                        $"{v.Brand} {v.Model}",
                        v.LicensePlate.Value,
                        m.Type.ToString(),
                        m.NextMaintenanceDateScheduled!.Value,
                        (m.NextMaintenanceDateScheduled.Value.Date - today).Days,
                        "NextFromRecord"
                    )));

            return scheduledAlerts.Concat(recordAlerts)
                .OrderBy(a => a.DueDate)
                .ToList();
        }
    }
}

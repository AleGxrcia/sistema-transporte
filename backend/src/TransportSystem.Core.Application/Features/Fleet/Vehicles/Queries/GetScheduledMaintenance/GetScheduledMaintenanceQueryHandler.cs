using MediatR;
using TransportSystem.Core.Application.Dtos.Maintenance;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetScheduledMaintenance
{
    public class GetScheduledMaintenanceQueryHandler
        : IRequestHandler<GetScheduledMaintenanceQuery, IReadOnlyList<ScheduledMaintenanceDto>>
    {
        private readonly IVehicleRepository _repository;

        public GetScheduledMaintenanceQueryHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<ScheduledMaintenanceDto>> Handle(
            GetScheduledMaintenanceQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetAllWithFullDetailsAsync(cancellationToken);

            var items = vehicles
                .SelectMany(v => v.ScheduledMaintenances
                    .Where(s => !request.OnlyPending || s.Status == ScheduledMaintenanceStatus.Pending)
                    .Select(s => new ScheduledMaintenanceDto(
                        s.Id,
                        v.Id,
                        v.LicensePlate.Value,
                        $"{v.Brand} {v.Model}",
                        s.Type.ToString(),
                        s.Description,
                        s.ScheduledDate,
                        s.Workshop,
                        s.ScheduledKm?.Value,
                        s.Status.ToString(),
                        s.DaysRemaining
                    )))
                .OrderBy(s => s.ScheduledDate)
                .ToList();

            return items;
        }
    }
}

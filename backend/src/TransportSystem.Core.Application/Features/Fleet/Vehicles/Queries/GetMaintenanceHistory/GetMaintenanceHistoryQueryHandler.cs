using MediatR;
using TransportSystem.Core.Application.Dtos.Maintenance;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetMaintenanceHistory
{
    public class GetMaintenanceHistoryQueryHandler
        : IRequestHandler<GetMaintenanceHistoryQuery, IReadOnlyList<MaintenanceHistoryItemDto>>
    {
        private readonly IVehicleRepository _repository;

        public GetMaintenanceHistoryQueryHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<MaintenanceHistoryItemDto>> Handle(
            GetMaintenanceHistoryQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetAllWithFullDetailsAsync(cancellationToken);

            if (request.VehicleId.HasValue)
                vehicles = vehicles.Where(v => v.Id == request.VehicleId.Value).ToList();

            var items = vehicles
                .SelectMany(v => v.MaintenanceRecords.Select(m => new MaintenanceHistoryItemDto(
                    m.Id,
                    v.Id,
                    v.LicensePlate.Value,
                    $"{v.Brand} {v.Model}",
                    m.Type.ToString(),
                    m.Description,
                    m.EntryDate,
                    m.EstimatedExitDate,
                    m.ActualExitDate,
                    m.Cost,
                    m.Workshop,
                    m.IsClosed,
                    m.NextMaintenanceDateScheduled
                )))
                .OrderByDescending(m => m.EntryDate)
                .ToList();

            return items;
        }
    }
}

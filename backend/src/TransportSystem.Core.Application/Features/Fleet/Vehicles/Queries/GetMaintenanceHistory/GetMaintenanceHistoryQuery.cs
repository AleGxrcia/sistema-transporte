using MediatR;
using TransportSystem.Core.Application.Dtos.Maintenance;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetMaintenanceHistory
{
    public record GetMaintenanceHistoryQuery(Guid? VehicleId = null)
        : IRequest<IReadOnlyList<MaintenanceHistoryItemDto>>;
}

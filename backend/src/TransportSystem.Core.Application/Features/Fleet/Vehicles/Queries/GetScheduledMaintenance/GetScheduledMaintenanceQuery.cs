using MediatR;
using TransportSystem.Core.Application.Dtos.Maintenance;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetScheduledMaintenance
{
    public record GetScheduledMaintenanceQuery(bool OnlyPending = true)
        : IRequest<IReadOnlyList<ScheduledMaintenanceDto>>;
}

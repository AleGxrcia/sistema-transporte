using MediatR;
using TransportSystem.Core.Application.Dtos.Maintenance;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetMaintenanceAlerts
{
    public record GetMaintenanceAlertsQuery(int WithinDays = 15)
        : IRequest<IReadOnlyList<MaintenanceAlertDto>>;
}

using MediatR;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ScheduleMaintenance
{
    public record ScheduleMaintenanceCommand(
        Guid VehicleId,
        MaintenanceType Type,
        string Description,
        DateTime ScheduledDate,
        string? Workshop,
        decimal? ScheduledKm
    ) : IRequest<Guid>;
}

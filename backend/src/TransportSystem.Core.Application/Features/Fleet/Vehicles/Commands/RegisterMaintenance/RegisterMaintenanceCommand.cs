using MediatR;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterMaintenance
{
    public record RegisterMaintenanceCommand(
        Guid VehicleId,
        MaintenanceType Type,
        string Description,
        DateTime EntryDate,
        string Workshop,
        DateTime? EstimatedExitDate,
        DateTime? NextMaintenanceDateScheduled,
        decimal? NextMaintenanceKmScheduled
    ) : IRequest<Guid>;
}

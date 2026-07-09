using MediatR;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.UpdateMaintenance
{
    public sealed record UpdateMaintenanceCommand(
        Guid VehicleId,
        Guid MaintenanceRecordId,
        MaintenanceType Type,
        string Description,
        DateTime EntryDate,
        string Workshop,
        DateTime? EstimatedExitDate,
        DateTime? NextMaintenanceDateScheduled,
        decimal? NextMaintenanceKmScheduled
    ) : IRequest;
}

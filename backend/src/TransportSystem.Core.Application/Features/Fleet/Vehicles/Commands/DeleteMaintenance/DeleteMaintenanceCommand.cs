using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.DeleteMaintenance
{
    public sealed record DeleteMaintenanceCommand(
        Guid VehicleId,
        Guid MaintenanceRecordId
    ) : IRequest;
}

using MediatR;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.ReassignVehicleAndDriver
{
    public record ReassignVehicleAndDriverCommand(
        Guid RequestId,
        Guid VehicleId,
        Guid DriverId
    ) : IRequest<Guid>;
}

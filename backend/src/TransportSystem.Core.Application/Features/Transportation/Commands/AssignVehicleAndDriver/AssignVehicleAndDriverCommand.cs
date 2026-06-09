using MediatR;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.AssignVehicleAndDriver
{
    public record AssignVehicleAndDriverCommand(
        Guid RequestId,
        Guid VehicleId,
        Guid DriverId
    ) : IRequest<Guid>;
}

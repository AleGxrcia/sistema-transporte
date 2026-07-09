using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.DeleteFuel
{
    public sealed record DeleteFuelCommand(
        Guid VehicleId,
        Guid FuelRecordId
    ) : IRequest;
}

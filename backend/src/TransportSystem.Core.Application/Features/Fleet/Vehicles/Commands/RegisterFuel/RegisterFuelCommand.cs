using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterFuel
{
    public sealed record RegisterFuelCommand(
        Guid VehicleId,
        DateTime RecordDate,
        decimal Gallons,
        decimal PricePerGallon,
        decimal MileageAtRefuel,
        string? Notes
    ) : IRequest<Guid>;
}

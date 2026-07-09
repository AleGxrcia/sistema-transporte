using MediatR;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.UpdateFuel
{
    public sealed record UpdateFuelCommand(
        Guid VehicleId,
        Guid FuelRecordId,
        DateTime RecordDate,
        decimal Gallons,
        decimal PricePerGallon,
        decimal MileageAtRefuel,
        string? Notes
    ) : IRequest;
}

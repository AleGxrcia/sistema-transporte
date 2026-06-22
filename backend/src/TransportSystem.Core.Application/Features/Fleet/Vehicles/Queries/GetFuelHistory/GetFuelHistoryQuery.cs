using MediatR;
using TransportSystem.Core.Application.Dtos.Fuel;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetFuelHistory
{
    public record GetFuelHistoryQuery(Guid? VehicleId = null, int? Year = null, int? Month = null)
        : IRequest<IReadOnlyList<FuelHistoryItemDto>>;
}

using MediatR;
using TransportSystem.Core.Application.Dtos.Vehicle;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetAvailableVehicles
{
    public record GetAvailableVehiclesQuery(int MinPassengers) 
        : IRequest<IReadOnlyList<AvailableVehicleDto>>;
}

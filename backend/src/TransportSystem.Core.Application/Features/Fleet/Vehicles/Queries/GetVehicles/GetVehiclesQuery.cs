using MediatR;
using TransportSystem.Core.Application.Dtos.Vehicle;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetVehicles
{
    public record GetVehiclesQuery(bool ArchivedOnly = false)
        : IRequest<IReadOnlyList<VehicleDto>>;
}

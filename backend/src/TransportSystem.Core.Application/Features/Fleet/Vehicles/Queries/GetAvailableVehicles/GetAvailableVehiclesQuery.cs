using MediatR;
using TransportSystem.Core.Application.Dtos.Vehicle;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetAvailableVehicles
{
    // From/To: ventana del viaje solicitado. Si se indican, excluye vehículos con una
    // asignación activa que se solape en ese horario.
    public record GetAvailableVehiclesQuery(int MinPassengers, DateTime? From = null, DateTime? To = null)
        : IRequest<IReadOnlyList<AvailableVehicleDto>>;
}

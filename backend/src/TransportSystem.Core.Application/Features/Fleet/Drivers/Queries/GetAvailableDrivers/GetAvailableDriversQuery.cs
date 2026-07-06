using MediatR;
using TransportSystem.Core.Application.Dtos.Driver;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetAvailableDrivers
{
    // From/To: ventana del viaje solicitado. Si se indican, excluye conductores con una
    // asignación activa que se solape en ese horario.
    public record GetAvailableDriversQuery(DateTime? From = null, DateTime? To = null)
        : IRequest<IReadOnlyList<AvailableDriverDto>>;
}

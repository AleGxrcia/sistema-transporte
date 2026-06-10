using MediatR;
using TransportSystem.Core.Application.Dtos.Driver;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetAvailableDrivers
{
    public record GetAvailableDriversQuery 
        : IRequest<IReadOnlyList<AvailableDriverDto>>;
}

using MediatR;
using TransportSystem.Core.Application.Dtos.Driver;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetDrivers
{
    public record GetDriversQuery 
        : IRequest<IReadOnlyList<DriverDto>>;
}

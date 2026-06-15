using MediatR;
using TransportSystem.Core.Application.Dtos.Driver;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetDriverById
{
    public record GetDriverByIdQuery(Guid Id)
        : IRequest<DriverDto>;
}

using MediatR;
using TransportSystem.Core.Application.Dtos.Vehicle;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetVehicleById
{
    public record GetVehicleByIdQuery(Guid Id) 
        : IRequest<VehicleDetailDto>;
}

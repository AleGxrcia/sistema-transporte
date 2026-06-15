using MediatR;
using TransportSystem.Core.Application.Dtos.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetAvailableVehicles
{
    public class GetAvailableVehiclesQueryHandler : IRequestHandler<GetAvailableVehiclesQuery, IReadOnlyList<AvailableVehicleDto>>
    {
        private readonly IVehicleRepository _repository;

        public GetAvailableVehiclesQueryHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<AvailableVehicleDto>> Handle(
            GetAvailableVehiclesQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetAvailableForAssignmentAsync(request.MinPassengers, cancellationToken);

            return vehicles.Select(v => new AvailableVehicleDto(
                v.Id, 
                v.LicensePlate.Value, 
                v.Brand, 
                v.Model, 
                v.Year, 
                v.Color, 
                v.Type.ToString(), 
                v.Capacity.Passengers
            )).ToList();
        }
    }
}

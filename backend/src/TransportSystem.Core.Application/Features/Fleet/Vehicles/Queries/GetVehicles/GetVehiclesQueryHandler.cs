using MediatR;
using TransportSystem.Core.Application.Dtos.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetVehicles
{
    public class GetVehiclesQueryHandler : IRequestHandler<GetVehiclesQuery, IReadOnlyList<VehicleDto>>
    {
        private readonly IVehicleRepository _repository;

        public GetVehiclesQueryHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<VehicleDto>> Handle(
            GetVehiclesQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetAllAsync(cancellationToken);

            var vehiclesDto = vehicles.Select(v => new VehicleDto(
                v.Id, v.LicensePlate.Value, v.Brand, v.Model, v.Year,
                v.Color, v.Type.ToString(), v.Capacity.Passengers,
                v.Status.ToString(), v.CurrentMileage.Value,
                v.LastMaintenanceDate, v.CreatedAt, v.UpdatedAt
            )).ToList();

            return vehiclesDto;
        }
    }
}

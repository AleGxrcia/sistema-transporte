using MediatR;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Application.Dtos.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetVehicles
{
    public class GetVehiclesQueryHandler : IRequestHandler<GetVehiclesQuery, IReadOnlyList<VehicleDto>>
    {
        private readonly IVehicleRepository _repository;
        private readonly ICurrentUser _currentUser;

        public GetVehiclesQueryHandler(IVehicleRepository repository, ICurrentUser currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<IReadOnlyList<VehicleDto>> Handle(
            GetVehiclesQuery request, CancellationToken cancellationToken)
        {
            if (request.ArchivedOnly && !_currentUser.IsAdmin)
                throw new ForbiddenException("ver vehículos archivados", "Administrador");

            var vehicles = request.ArchivedOnly
                ? await _repository.GetArchivedAsync(cancellationToken)
                : await _repository.GetAllAsync(cancellationToken);

            var vehiclesDto = vehicles.Select(v => new VehicleDto(
                v.Id, v.LicensePlate.Value, v.Brand, v.Model, v.Year,
                v.Color, v.Type.ToString(), v.Capacity.Passengers,
                v.Status.ToString(), v.CurrentMileage.Value,
                v.LastMaintenanceDate, v.CreatedAt, v.UpdatedAt,
                v.IsDeleted, v.DeletedAt
            )).ToList();

            return vehiclesDto;
        }
    }
}

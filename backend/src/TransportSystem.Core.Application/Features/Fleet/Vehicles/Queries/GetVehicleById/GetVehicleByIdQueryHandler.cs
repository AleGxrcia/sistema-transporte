using MediatR;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Dtos.Fuel;
using TransportSystem.Core.Application.Dtos.Maintenance;
using TransportSystem.Core.Application.Dtos.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetVehicleById
{
    public class GetVehicleByIdQueryHandler : IRequestHandler<GetVehicleByIdQuery, VehicleDetailDto>
    {
        private readonly IVehicleRepository _repository;

        public GetVehicleByIdQueryHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<VehicleDetailDto> Handle(
            GetVehicleByIdQuery request, CancellationToken cancellationToken)
        {
            var vehicle = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Vehículo", request.Id);

            var maintenance = vehicle.MaintenanceRecords.Select(m => new MaintenanceRecordDto(
                m.Id, m.Type.ToString(), m.Description, m.EntryDate,
                m.EstimatedExitDate, m.ActualExitDate, m.Cost, m.Workshop, m.IsClosed,
                m.NextMaintenanceDateScheduled, m.NextMaintenanceKmScheduled?.Value
            )).ToList();

            var fuel = vehicle.FuelRecords.Select(f => new FuelRecordDto(
                f.Id, f.RecordDate, f.Gallons, f.PricePerGallon,
                f.TotalCost, f.MileageAtRefuel.Value, f.Notes
            )).ToList();

            return new VehicleDetailDto(
                vehicle.Id, 
                vehicle.LicensePlate.Value, 
                vehicle.Brand, 
                vehicle.Model, 
                vehicle.Year,
                vehicle.Color, 
                vehicle.Type.ToString(), 
                vehicle.Capacity.Passengers,
                vehicle.Status.ToString(), 
                vehicle.CurrentMileage.Value,
                vehicle.LastMaintenanceDate,
                maintenance,
                fuel,
                vehicle.CreatedAt,
                vehicle.IsDeleted,
                vehicle.DeletedAt
            );
        }
    }
}

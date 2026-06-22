using MediatR;
using TransportSystem.Core.Application.Dtos.Fuel;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetFuelHistory
{
    public class GetFuelHistoryQueryHandler
        : IRequestHandler<GetFuelHistoryQuery, IReadOnlyList<FuelHistoryItemDto>>
    {
        private readonly IVehicleRepository _repository;

        public GetFuelHistoryQueryHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<FuelHistoryItemDto>> Handle(
            GetFuelHistoryQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetAllWithFullDetailsAsync(cancellationToken);

            if (request.VehicleId.HasValue)
                vehicles = vehicles.Where(v => v.Id == request.VehicleId.Value).ToList();

            var items = new List<FuelHistoryItemDto>();

            foreach (var vehicle in vehicles)
            {
                var orderedRecords = vehicle.FuelRecords.OrderBy(f => f.RecordDate).ToList();

                for (var i = 0; i < orderedRecords.Count; i++)
                {
                    var record = orderedRecords[i];

                    decimal? kmDriven = null;
                    decimal? efficiency = null;

                    if (i > 0)
                    {
                        kmDriven = record.MileageAtRefuel.Value - orderedRecords[i - 1].MileageAtRefuel.Value;
                        if (record.Gallons > 0 && kmDriven.Value > 0)
                            efficiency = Math.Round(kmDriven.Value / record.Gallons, 1);
                    }

                    items.Add(new FuelHistoryItemDto(
                        record.Id,
                        vehicle.Id,
                        vehicle.LicensePlate.Value,
                        $"{vehicle.Brand} {vehicle.Model}",
                        record.RecordDate,
                        record.Gallons,
                        record.PricePerGallon,
                        record.TotalCost,
                        record.MileageAtRefuel.Value,
                        record.Notes,
                        kmDriven,
                        efficiency
                    ));
                }
            }

            var filtered = items.Where(i =>
                (!request.Year.HasValue || i.RecordDate.Year == request.Year.Value) &&
                (!request.Month.HasValue || i.RecordDate.Month == request.Month.Value));

            return filtered.OrderByDescending(i => i.RecordDate).ToList();
        }
    }
}

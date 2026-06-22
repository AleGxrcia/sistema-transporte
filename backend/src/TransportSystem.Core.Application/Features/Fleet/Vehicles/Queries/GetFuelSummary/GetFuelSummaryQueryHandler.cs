using MediatR;
using TransportSystem.Core.Application.Dtos.Fuel;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetFuelSummary
{
    public class GetFuelSummaryQueryHandler : IRequestHandler<GetFuelSummaryQuery, FuelSummaryDto>
    {
        private readonly IVehicleRepository _repository;

        public GetFuelSummaryQueryHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<FuelSummaryDto> Handle(GetFuelSummaryQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetAllWithFullDetailsAsync(cancellationToken);

            var consumptionByVehicle = vehicles
                .Select(v => new
                {
                    Vehicle = v,
                    Records = v.FuelRecords.Where(f =>
                        f.RecordDate.Year == request.Year && f.RecordDate.Month == request.Month).ToList()
                })
                .Where(x => x.Records.Count > 0)
                .Select(x => new FuelConsumptionByVehicleDto(
                    x.Vehicle.Id,
                    $"{x.Vehicle.Brand} {x.Vehicle.Model}",
                    x.Vehicle.LicensePlate.Value,
                    x.Records.Sum(r => r.Gallons),
                    x.Records.Sum(r => r.TotalCost)
                ))
                .OrderByDescending(c => c.Gallons)
                .ToList();

            var totalGallons = consumptionByVehicle.Sum(c => c.Gallons);
            var totalCost = consumptionByVehicle.Sum(c => c.TotalCost);

            return new FuelSummaryDto(
                request.Year,
                request.Month,
                totalGallons,
                totalCost,
                totalGallons > 0 ? Math.Round(totalCost / totalGallons, 2) : 0,
                consumptionByVehicle.Count,
                vehicles.Count,
                consumptionByVehicle.FirstOrDefault(),
                consumptionByVehicle
            );
        }
    }
}

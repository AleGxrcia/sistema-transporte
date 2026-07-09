using MediatR;
using TransportSystem.Core.Application.Dtos.Vehicle;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetAvailableVehicles
{
    public class GetAvailableVehiclesQueryHandler : IRequestHandler<GetAvailableVehiclesQuery, IReadOnlyList<AvailableVehicleDto>>
    {
        private readonly IVehicleRepository _repository;
        private readonly IScheduleRepository _scheduleRepository;

        public GetAvailableVehiclesQueryHandler(IVehicleRepository repository, IScheduleRepository scheduleRepository)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<IReadOnlyList<AvailableVehicleDto>> Handle(
            GetAvailableVehiclesQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _repository.GetAvailableForAssignmentAsync(request.MinPassengers, cancellationToken);

            if (request.From.HasValue && request.To.HasValue)
            {
                var schedule = await _scheduleRepository.GetByDateWithAssignmentsAsync(request.From.Value, cancellationToken);
                if (schedule is not null)
                {
                    var busyVehicleIds = schedule.Assignments
                        .Where(a => a.IsActive()
                            && request.From.Value < a.TimeSlot.ReturnTime
                            && a.TimeSlot.DepartureTime < request.To.Value)
                        .Select(a => a.VehicleId)
                        .ToHashSet();

                    vehicles = vehicles.Where(v => !busyVehicleIds.Contains(v.Id)).ToList();
                }
            }

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

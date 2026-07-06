using MediatR;
using TransportSystem.Core.Application.Dtos.Driver;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetAvailableDrivers
{
    public class GetAvailableDriversQueryHandler : IRequestHandler<GetAvailableDriversQuery, IReadOnlyList<AvailableDriverDto>>
    {
        private readonly IDriverRepository _repository;
        private readonly IScheduleRepository _scheduleRepository;

        public GetAvailableDriversQueryHandler(IDriverRepository repository, IScheduleRepository scheduleRepository)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<IReadOnlyList<AvailableDriverDto>> Handle(
            GetAvailableDriversQuery request, CancellationToken cancellationToken)
        {
            var drivers = await _repository.GetAvailableForAssignmentAsync(cancellationToken);

            if (request.From.HasValue && request.To.HasValue)
            {
                var schedule = await _scheduleRepository.GetByDateWithAssignmentsAsync(request.From.Value, cancellationToken);
                if (schedule is not null)
                {
                    var busyDriverIds = schedule.Assignments
                        .Where(a => a.IsActive()
                            && request.From.Value < a.TimeSlot.ReturnTime
                            && a.TimeSlot.DepartureTime < request.To.Value)
                        .Select(a => a.DriverId)
                        .ToHashSet();

                    drivers = drivers.Where(d => !busyDriverIds.Contains(d.Id)).ToList();
                }
            }

            return drivers.Select(d => new AvailableDriverDto(
                d.Id,
                d.FirstName,
                d.LastName,
                d.License.Number,
                d.License.Category.ToString(),
                d.License.ExpirationDate
            )).ToList();
        }
    }
}

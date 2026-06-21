using MediatR;
using TransportSystem.Core.Application.Dtos.Assignment;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Entities;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetScheduleByDate
{
    public class GetScheduleByDateQueryHandler : IRequestHandler<GetScheduleByDateQuery, IReadOnlyList<AssignmentDto>>
    {
        private readonly IScheduleRepository _repository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ITravelRequestRepository _requestRepository;

        public GetScheduleByDateQueryHandler(IScheduleRepository repository, IVehicleRepository vehicleRepository,
            IDriverRepository driverRepository, ITravelRequestRepository requestRepository)
        {
            _repository = repository;
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _requestRepository = requestRepository;
        }

        public async Task<IReadOnlyList<AssignmentDto>> Handle(GetScheduleByDateQuery request, CancellationToken cancellationToken)
        {
            var assignments = new List<Assignment>();

            for (var date = request.From.Date; date <= request.To.Date; date = date.AddDays(1))
            {
                var schedule = await _repository.GetByDateWithAssignmentsAsync(date, cancellationToken);
                if (schedule is null)
                    continue;

                assignments.AddRange(schedule.Assignments.Where(a =>
                    a.TimeSlot.DepartureTime >= request.From && a.TimeSlot.DepartureTime <= request.To));
            }

            var result = new List<AssignmentDto>();

            foreach (var assignment in assignments)
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(assignment.VehicleId, cancellationToken);
                var driver = await _driverRepository.GetByIdAsync(assignment.DriverId, cancellationToken);
                var travelRequest = await _requestRepository.GetByIdAsync(assignment.RequestId, cancellationToken);

                result.Add(new AssignmentDto(
                    AssignmentId: assignment.Id,
                    RequestId: assignment.RequestId,
                    RequestNumber: travelRequest?.RequestNumber.Value ?? string.Empty,
                    VehiclePlate: vehicle?.LicensePlate.Value ?? string.Empty,
                    VehicleDescription: vehicle is null ? string.Empty : $"{vehicle.Brand} {vehicle.Model}",
                    DriverFullName: driver is null ? string.Empty : $"{driver.FirstName} {driver.LastName}",
                    DepartureTime: assignment.TimeSlot.DepartureTime,
                    ReturnTime: assignment.TimeSlot.ReturnTime,
                    Destination: travelRequest?.Destination.Name ?? string.Empty,
                    Status: assignment.Status.ToString(),
                    CancellationReason: assignment.CancellationReason,
                    ActualDepartureTime: assignment.ActualDepartureTime,
                    ActualReturnTime: assignment.ActualReturnTime
                ));
            }

            return result;
        }
    }
}

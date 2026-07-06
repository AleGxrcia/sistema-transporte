using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Application.Dtos.TravelRequest;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetAllRequests
{
    public class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, IReadOnlyList<TravelRequestDto>>
    {
        private readonly ITravelRequestRepository _repository;
        private readonly ICurrentUser _currentUser;

        public GetAllRequestsQueryHandler(ITravelRequestRepository repository, ICurrentUser currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<IReadOnlyList<TravelRequestDto>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
        {
            var isSupervisorOrAdmin = _currentUser.IsInAnyRole(UserRole.Admin, UserRole.Supervisor);

            var all = await _repository.GetAllAsync(cancellationToken);

            var visible = isSupervisorOrAdmin
                ? all
                : all.Where(r => r.RequestedByUserId == _currentUser.Id);

            return visible
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new TravelRequestDto(
                    r.Id,
                    r.RequestNumber.Value,
                    r.RequestingArea,
                    r.PassengerCount,
                    r.Destination.Name,
                    r.RequestedTimeSlot.DepartureTime,
                    r.RequestedTimeSlot.ReturnTime,
                    r.TripPurpose,
                    r.Status.ToString(),
                    r.CreatedAt
                )).ToList();
        }
    }
}

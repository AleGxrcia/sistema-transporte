using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Application.Dtos.TravelRequest;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetPendingRequests
{
    public class GetPendingRequestsQueryHandler : IRequestHandler<GetPendingRequestsQuery, IReadOnlyList<TravelRequestDto>>
    {
        private readonly ITravelRequestRepository _repository;
        private readonly ICurrentUser _currentUser;

        public GetPendingRequestsQueryHandler(ITravelRequestRepository repository, ICurrentUser currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<IReadOnlyList<TravelRequestDto>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
        {
            var isSupervisorOrAdmin =
                _currentUser.IsInRole(UserRole.Supervisor) || _currentUser.IsInRole(UserRole.Admin);

            // TODO: Si el usuario es admin o supervisor, debería poder ver todas las solicitudes pendientes, no solo las suyas

            var result = await _repository.GetAllAsync(cancellationToken);

            return result.Select(r => new TravelRequestDto(
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

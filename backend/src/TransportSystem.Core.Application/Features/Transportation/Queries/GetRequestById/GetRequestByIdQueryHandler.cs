using MediatR;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Dtos.TravelRequest;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetRequestById
{
    public class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByIdQuery, TravelRequestDetailDto>
    {
        private readonly ITravelRequestRepository _repository;

        public GetRequestByIdQueryHandler(ITravelRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<TravelRequestDetailDto> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var req = await _repository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Solicitud", request.Id);

            return new TravelRequestDetailDto(
                req.Id, 
                req.RequestNumber.Value, 
                req.RequestingArea,
                req.PassengerCount, 
                req.Destination.Name,
                req.RequestedTimeSlot.DepartureTime,
                req.RequestedTimeSlot.ReturnTime,
                req.TripPurpose, 
                req.Status.ToString(),
                req.RequestedByUserId, 
                req.ApprovedByUserId, 
                req.ApprovedAt,
                req.RejectionReason, 
                req.CancellationReason,
                req.CancelledByUserId, 
                req.CancelledAt,
                req.AssignedVehicleId, 
                req.AssignedDriverId,
                req.CreatedAt,
                req.UpdatedAt
            );
        }
    }
}

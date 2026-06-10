using MediatR;
using TransportSystem.Core.Application.Dtos.TravelRequest;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetPendingRequests
{
    public sealed record GetPendingRequestsQuery
        : IRequest<IReadOnlyList<TravelRequestDto>>;
}

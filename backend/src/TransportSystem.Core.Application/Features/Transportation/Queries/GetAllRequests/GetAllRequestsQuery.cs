using MediatR;
using TransportSystem.Core.Application.Dtos.TravelRequest;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetAllRequests
{
    public record GetAllRequestsQuery() : IRequest<IReadOnlyList<TravelRequestDto>>;
}

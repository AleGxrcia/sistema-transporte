using MediatR;
using TransportSystem.Core.Application.Dtos.TravelRequest;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetRequestById
{
    public record GetRequestByIdQuery(Guid Id) 
        : IRequest<TravelRequestDetailDto>;
}

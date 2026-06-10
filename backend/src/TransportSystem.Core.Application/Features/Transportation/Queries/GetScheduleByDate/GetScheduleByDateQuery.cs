using MediatR;
using TransportSystem.Core.Application.Dtos.Assignment;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetScheduleByDate
{
    public record GetScheduleByDateQuery(DateTime From, DateTime To)
        : IRequest<IReadOnlyList<AssignmentDto>>;
}

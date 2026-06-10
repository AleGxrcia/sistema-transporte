using MediatR;
using TransportSystem.Core.Application.Dtos.Assignment;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetScheduleByDate
{
    public class GetScheduleByDateQueryHandler : IRequestHandler<GetScheduleByDateQuery, IReadOnlyList<AssignmentDto>>
    {
        private readonly IScheduleRepository _repository;

        public GetScheduleByDateQueryHandler(IScheduleRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<AssignmentDto>> Handle(GetScheduleByDateQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

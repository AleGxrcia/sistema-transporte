using MediatR;
using TransportSystem.Core.Application.Dtos.Driver;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetAvailableDrivers
{
    public class GetAvailableDriversQueryHandler : IRequestHandler<GetAvailableDriversQuery, IReadOnlyList<AvailableDriverDto>>
    {
        private readonly IDriverRepository _repository;

        public GetAvailableDriversQueryHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<AvailableDriverDto>> Handle(
            GetAvailableDriversQuery request, CancellationToken cancellationToken)
        {
            var drivers = await _repository.GetAvailableForAssignmentAsync(cancellationToken);

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

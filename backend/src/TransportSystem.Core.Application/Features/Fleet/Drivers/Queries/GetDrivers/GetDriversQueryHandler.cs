using MediatR;
using TransportSystem.Core.Application.Dtos.Driver;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetDrivers
{
    public class GetDriversQueryHandler : IRequestHandler<GetDriversQuery, IReadOnlyList<DriverDto>>
    {
        private readonly IDriverRepository _repository;

        public GetDriversQueryHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<DriverDto>> Handle(
            GetDriversQuery request, CancellationToken cancellationToken)
        {
            var drivers = await _repository.GetAllAsync(cancellationToken);

            var driversDto = drivers.Select(d => new DriverDto(
                d.Id,
                d.FirstName, 
                d.LastName,
                d.NationalId.Value, 
                d.License.Number,
                d.License.Category.ToString(), 
                d.License.ExpirationDate,
                d.License.IsExpired(), 
                d.License.ExpiresWithin(30),
                d.Phone, 
                d.Address, 
                d.Status.ToString(),
                d.SupervisorId, 
                d.CreatedAt, 
                d.UpdatedAt
            )).ToList();

            return driversDto;
        }
    }
}

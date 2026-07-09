using MediatR;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Dtos.Driver;
using TransportSystem.Core.Domain.Fleet.Repositories;

namespace TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetDriverById
{
    public class GetDriverByIdQueryHandler : IRequestHandler<GetDriverByIdQuery, DriverDto>
    {
        private readonly IDriverRepository _repository;

        public GetDriverByIdQueryHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<DriverDto> Handle(
            GetDriverByIdQuery request, CancellationToken cancellationToken)
        {
            var driver = await _repository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Conductor", request.Id);

            return new DriverDto(
                driver.Id,
                driver.FirstName, 
                driver.LastName,
                driver.NationalId.Value, 
                driver.License.Number,
                driver.License.Category.ToString(), 
                driver.License.ExpirationDate,
                driver.License.IsExpired(), 
                driver.License.ExpiresWithin(30),
                driver.Phone, 
                driver.Address, 
                driver.Status.ToString(),
                driver.SupervisorId,
                driver.CreatedAt,
                driver.UpdatedAt,
                driver.IsDeleted,
                driver.DeletedAt
            );
        }
    }
}

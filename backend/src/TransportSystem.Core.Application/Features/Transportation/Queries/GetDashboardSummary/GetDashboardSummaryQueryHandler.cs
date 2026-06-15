using MediatR;
using TransportSystem.Core.Application.Dtos.Dashboard;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetDashboardSummary
{
    public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ITravelRequestRepository _requestRepository;

        public GetDashboardSummaryQueryHandler(IVehicleRepository vehicleRepository, IDriverRepository driverRepository,
            ITravelRequestRepository requestRepository)
        {
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _requestRepository = requestRepository;
        }

        public Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

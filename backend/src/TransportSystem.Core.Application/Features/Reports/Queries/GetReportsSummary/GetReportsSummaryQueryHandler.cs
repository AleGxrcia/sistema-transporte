using MediatR;
using TransportSystem.Core.Application.Dtos.Reports;
using TransportSystem.Core.Application.Features.Reports.Common;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Reports.Queries.GetReportsSummary
{
    public class GetReportsSummaryQueryHandler : IRequestHandler<GetReportsSummaryQuery, ReportsSummaryDto>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ITravelRequestRepository _requestRepository;

        public GetReportsSummaryQueryHandler(IVehicleRepository vehicleRepository, IDriverRepository driverRepository,
            ITravelRequestRepository requestRepository)
        {
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _requestRepository = requestRepository;
        }

        public async Task<ReportsSummaryDto> Handle(GetReportsSummaryQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _vehicleRepository.GetAllWithFullDetailsAsync(cancellationToken);
            var drivers = await _driverRepository.GetAllAsync(cancellationToken);
            var requests = await _requestRepository.GetAllAsync(cancellationToken);

            return ReportsAggregator.BuildSummary(vehicles, drivers, requests, request.Year, request.Month);
        }
    }
}

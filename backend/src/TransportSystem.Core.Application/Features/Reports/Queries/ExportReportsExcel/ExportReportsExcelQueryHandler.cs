using MediatR;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Application.Features.Reports.Common;
using TransportSystem.Core.Domain.Fleet.Repositories;
using TransportSystem.Core.Domain.Transportation.Repositories;

namespace TransportSystem.Core.Application.Features.Reports.Queries.ExportReportsExcel
{
    public class ExportReportsExcelQueryHandler : IRequestHandler<ExportReportsExcelQuery, byte[]>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ITravelRequestRepository _requestRepository;
        private readonly IReportExportService _exportService;

        public ExportReportsExcelQueryHandler(IVehicleRepository vehicleRepository, IDriverRepository driverRepository,
            ITravelRequestRepository requestRepository, IReportExportService exportService)
        {
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _requestRepository = requestRepository;
            _exportService = exportService;
        }

        public async Task<byte[]> Handle(ExportReportsExcelQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _vehicleRepository.GetAllWithFullDetailsAsync(cancellationToken);
            var drivers = await _driverRepository.GetAllAsync(cancellationToken);
            var requests = await _requestRepository.GetAllAsync(cancellationToken);

            var summary = ReportsAggregator.BuildSummary(vehicles, drivers, requests, request.Year, request.Month);

            return _exportService.GenerateExcel(summary);
        }
    }
}

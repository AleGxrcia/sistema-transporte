using TransportSystem.Core.Application.Dtos.Reports;

namespace TransportSystem.Core.Application.Common.Interfaces
{
    public interface IReportExportService
    {
        byte[] GenerateExcel(ReportsSummaryDto report);
        byte[] GeneratePdf(ReportsSummaryDto report);
    }
}

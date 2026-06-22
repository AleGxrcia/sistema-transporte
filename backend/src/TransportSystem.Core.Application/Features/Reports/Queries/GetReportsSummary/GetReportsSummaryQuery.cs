using MediatR;
using TransportSystem.Core.Application.Dtos.Reports;

namespace TransportSystem.Core.Application.Features.Reports.Queries.GetReportsSummary
{
    public record GetReportsSummaryQuery(int Year, int Month) : IRequest<ReportsSummaryDto>;
}

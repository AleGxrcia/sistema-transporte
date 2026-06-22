using MediatR;

namespace TransportSystem.Core.Application.Features.Reports.Queries.ExportReportsPdf
{
    public record ExportReportsPdfQuery(int Year, int Month) : IRequest<byte[]>;
}

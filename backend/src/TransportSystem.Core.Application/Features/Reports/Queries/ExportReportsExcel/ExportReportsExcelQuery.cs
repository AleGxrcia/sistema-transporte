using MediatR;

namespace TransportSystem.Core.Application.Features.Reports.Queries.ExportReportsExcel
{
    public record ExportReportsExcelQuery(int Year, int Month) : IRequest<byte[]>;
}

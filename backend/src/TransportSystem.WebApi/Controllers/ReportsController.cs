using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Dtos.Reports;
using TransportSystem.Core.Application.Features.Reports.Queries.ExportReportsExcel;
using TransportSystem.Core.Application.Features.Reports.Queries.ExportReportsPdf;
using TransportSystem.Core.Application.Features.Reports.Queries.GetReportsSummary;

namespace TransportSystem.WebApi.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/reports")]
    public class ReportsController : ApiControllerBase
    {
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReportsSummaryDto))]
        public async Task<IActionResult> GetSummary(
            [FromQuery] int year,
            [FromQuery] int month,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetReportsSummaryQuery(year, month), cancellationToken);
            return Ok(result);
        }

        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExcel(
            [FromQuery] int year,
            [FromQuery] int month,
            CancellationToken cancellationToken)
        {
            var bytes = await Sender.Send(new ExportReportsExcelQuery(year, month), cancellationToken);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"reporte-{year}-{month:D2}.xlsx");
        }

        [HttpGet("export/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportPdf(
            [FromQuery] int year,
            [FromQuery] int month,
            CancellationToken cancellationToken)
        {
            var bytes = await Sender.Send(new ExportReportsPdfQuery(year, month), cancellationToken);
            return File(bytes, "application/pdf", $"reporte-{year}-{month:D2}.pdf");
        }
    }
}

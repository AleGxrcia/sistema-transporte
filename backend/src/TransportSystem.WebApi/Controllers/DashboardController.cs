using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Dtos.Dashboard;
using TransportSystem.Core.Application.Features.Transportation.Queries.GetDashboardSummary;

namespace TransportSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/dashboard")]
    public class DashboardController : ApiControllerBase
    {
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DashboardSummaryDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetDashboardSummaryQuery(), cancellationToken);
            return Ok(result);
        }
    }
}

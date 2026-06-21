using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Dtos.Assignment;
using TransportSystem.Core.Application.Features.Transportation.Queries.GetScheduleByDate;

namespace TransportSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/schedule")]
    public class ScheduleController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<AssignmentDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetScheduleByDateQuery(from, to), cancellationToken);
            return Ok(result);
        }
    }
}

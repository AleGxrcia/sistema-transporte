using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Dtos.Fuel;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetFuelHistory;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetFuelSummary;

namespace TransportSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/fuel")]
    public class FuelController : ApiControllerBase
    {
        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<FuelHistoryItemDto>))]
        public async Task<IActionResult> GetHistory(
            [FromQuery] Guid? vehicleId,
            [FromQuery] int? year,
            [FromQuery] int? month,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetFuelHistoryQuery(vehicleId, year, month), cancellationToken);
            return Ok(result);
        }

        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FuelSummaryDto))]
        public async Task<IActionResult> GetSummary(
            [FromQuery] int year,
            [FromQuery] int month,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetFuelSummaryQuery(year, month), cancellationToken);
            return Ok(result);
        }
    }
}

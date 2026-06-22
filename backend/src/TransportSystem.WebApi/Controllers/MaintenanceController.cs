using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Dtos.Maintenance;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.CancelScheduledMaintenance;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ExecuteScheduledMaintenance;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.ScheduleMaintenance;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetMaintenanceAlerts;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetMaintenanceHistory;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetScheduledMaintenance;
using TransportSystem.WebApi.Contracts.Maintenance;

namespace TransportSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/maintenance")]
    public class MaintenanceController : ApiControllerBase
    {
        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<MaintenanceHistoryItemDto>))]
        public async Task<IActionResult> GetHistory(
            [FromQuery] Guid? vehicleId,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetMaintenanceHistoryQuery(vehicleId), cancellationToken);
            return Ok(result);
        }

        [HttpGet("scheduled")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<ScheduledMaintenanceDto>))]
        public async Task<IActionResult> GetScheduled(
            [FromQuery] bool onlyPending = true,
            CancellationToken cancellationToken = default)
        {
            var result = await Sender.Send(new GetScheduledMaintenanceQuery(onlyPending), cancellationToken);
            return Ok(result);
        }

        [HttpGet("alerts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<MaintenanceAlertDto>))]
        public async Task<IActionResult> GetAlerts(
            [FromQuery] int withinDays = 15,
            CancellationToken cancellationToken = default)
        {
            var result = await Sender.Send(new GetMaintenanceAlertsQuery(withinDays), cancellationToken);
            return Ok(result);
        }

        [HttpPost("scheduled")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Schedule(
            [FromBody] ScheduleMaintenanceRequest body,
            CancellationToken cancellationToken)
        {
            var id = await Sender.Send(new ScheduleMaintenanceCommand(
                body.VehicleId, body.Type, body.Description, body.ScheduledDate,
                body.Workshop, body.ScheduledKm), cancellationToken);

            return CreatedAtRoute("GetVehicleById", new { id = body.VehicleId }, new { scheduledMaintenanceId = id });
        }

        [HttpPatch("scheduled/{vehicleId:guid}/{id:guid}/cancel")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CancelScheduled(
            Guid vehicleId,
            Guid id,
            [FromBody] CancelScheduledMaintenanceRequest body,
            CancellationToken cancellationToken)
        {
            await Sender.Send(new CancelScheduledMaintenanceCommand(vehicleId, id, body.Reason), cancellationToken);
            return NoContent();
        }

        [HttpPost("scheduled/{vehicleId:guid}/{id:guid}/execute")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ExecuteScheduled(
            Guid vehicleId,
            Guid id,
            [FromBody] ExecuteScheduledMaintenanceRequest body,
            CancellationToken cancellationToken)
        {
            var recordId = await Sender.Send(new ExecuteScheduledMaintenanceCommand(
                vehicleId, id, body.EntryDate, body.Workshop, body.EstimatedExitDate,
                body.NextMaintenanceDateScheduled, body.NextMaintenanceKmScheduled), cancellationToken);

            return CreatedAtRoute("GetVehicleById", new { id = vehicleId }, new { maintenanceRecordId = recordId });
        }
    }
}

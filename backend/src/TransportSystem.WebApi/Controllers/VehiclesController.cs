using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Dtos.Vehicle;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.CloseMaintenance;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.DeactivateVehicle;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.DeleteVehicle;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterFuel;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterMaintenance;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.RegisterVehicle;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.UpdateVehicle;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetAvailableVehicles;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetVehicleById;
using TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetVehicles;
using TransportSystem.WebApi.Contracts.Vehicles;

namespace TransportSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/vehicles")]
    public class VehiclesController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<VehicleDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await Sender.Send(
                new GetVehiclesQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("available")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<AvailableVehicleDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailable(
            [FromQuery] int minPassengers = 1,
            CancellationToken cancellationToken = default)
        {
            var result = await Sender.Send(new GetAvailableVehiclesQuery(minPassengers), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}", Name = "GetVehicleById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VehicleDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetVehicleByIdQuery(id), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterVehicleCommand cmd,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(cmd, cancellationToken);
            return CreatedAtRoute("GetVehicleById", new { id = result }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateVehicleRequest body,
            CancellationToken cancellationToken)
        {
            await Sender.Send(new UpdateVehicleCommand(
                id, body.Brand, body.Model, body.Color, body.Type), cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await Sender.Send(new DeleteVehicleCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            await Sender.Send(new DeactivateVehicleCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/maintenance")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RegisterMaintenance(
            Guid id,
            [FromBody] RegisterMaintenanceRequest body,
            CancellationToken cancellationToken)
        {
            var recordId = await Sender.Send(new RegisterMaintenanceCommand(
                id, body.Type, body.Description, body.EntryDate,
                body.Workshop, body.EstimatedExitDate,
                body.NextMaintenanceDateScheduled, body.NextMaintenanceKmScheduled), cancellationToken);

            return CreatedAtRoute("GetVehicleById", new { id }, new { maintenanceRecordId = recordId });
        }

        [HttpPatch("{id:guid}/maintenance/{recordId:guid}/close")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CloseMaintenance(
            Guid id,
            Guid recordId,
            [FromBody] CloseMaintenanceRequest body,
            CancellationToken cancellationToken)
        {
            await Sender.Send(new CloseMaintenanceRecordCommand(
                id, recordId, body.ActualExitDate, body.Cost), cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/fuel")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RegisterFuel(
            Guid id,
            [FromBody] RegisterFuelRequest body,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new RegisterFuelCommand(
                id, body.RecordDate, body.Gallons,
                body.PricePerGallon, body.MileageAtRefuel, body.Notes), cancellationToken);

            return CreatedAtRoute("GetVehicleById", new { id }, result);
        }
    }
}

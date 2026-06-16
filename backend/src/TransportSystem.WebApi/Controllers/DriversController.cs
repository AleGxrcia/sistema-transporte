using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Dtos.Driver;
using TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.DeleteDriver;
using TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.ReactivateDriver;
using TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RegisterDriver;
using TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.RenewLicense;
using TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.SuspendDriver;
using TransportSystem.Core.Application.Features.Fleet.Drivers.Commands.UpdateDriver;
using TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetAvailableDrivers;
using TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetDriverById;
using TransportSystem.Core.Application.Features.Fleet.Drivers.Queries.GetDrivers;
using TransportSystem.WebApi.Contracts.Drivers;

namespace TransportSystem.WebApi.Controllers
{
    [Route("api/drivers")]
    public class DriversController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<DriverDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Sender.Send(new GetDriversQuery());
            return Ok(result);
        }

        [HttpGet("available")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<AvailableDriverDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailable(CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetAvailableDriversQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}", Name = "GetDriverById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DriverDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetDriverByIdQuery(id), cancellationToken);
            return Ok(result);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDriverCommand cmd, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(cmd, cancellationToken);
            return CreatedAtRoute("GetDriverById", new { result }, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(
            Guid id, [FromBody] UpdateDriverRequest body, CancellationToken cancellationToken)
        {
            await Sender.Send(new UpdateDriverCommand(id, body.Phone, body.Address, body.SupervisorId), cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await Sender.Send(new DeleteDriverCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/suspend")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Suspend(
            Guid id, [FromBody] SuspendRequest body, CancellationToken cancellationToken)
        {
            await Sender.Send(new SuspendDriverCommand(id, body.Reason), cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/reactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Reactivate(Guid id, CancellationToken cancellationToken)
        {
            await Sender.Send(new ReactivateDriverCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/license")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RenewLicense(
            Guid id, [FromBody] RenewLicenseRequest body, CancellationToken cancellationToken)
        {
            await Sender.Send(new RenewLicenseCommand(
                id, body.LicenseNumber, body.Category, body.ExpirationDate), cancellationToken);

            return NoContent();
        }
    }
}

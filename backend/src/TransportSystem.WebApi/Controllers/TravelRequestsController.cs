using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Dtos.TravelRequest;
using TransportSystem.Core.Application.Features.Transportation.Commands.ApproveRequest;
using TransportSystem.Core.Application.Features.Transportation.Commands.AssignVehicleAndDriver;
using TransportSystem.Core.Application.Features.Transportation.Commands.CancelRequest;
using TransportSystem.Core.Application.Features.Transportation.Commands.CompleteTrip;
using TransportSystem.Core.Application.Features.Transportation.Commands.CreateTravelRequest;
using TransportSystem.Core.Application.Features.Transportation.Commands.RejectRequest;
using TransportSystem.Core.Application.Features.Transportation.Commands.StartTrip;
using TransportSystem.Core.Application.Features.Transportation.Queries.GetPendingRequests;
using TransportSystem.Core.Application.Features.Transportation.Queries.GetRequestById;
using TransportSystem.WebApi.Contracts.Requests;

namespace TransportSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/requests")]
    public class TravelRequestsController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<TravelRequestDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetPendingRequestsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}", Name = "GetRequestById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TravelRequestDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetRequestByIdQuery(id), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create(
            [FromBody] CreateTravelRequestCommand cmd,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(cmd, cancellationToken);
            return CreatedAtRoute("GetRequestById", new { id = result }, result);
        }

        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
        {
            await Sender.Send(new ApproveRequestCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Reject(
            Guid id, [FromBody] RejectRequestBody body, CancellationToken cancellationToken)
        {
            await Sender.Send(new RejectRequestCommand(id, body.Reason), cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/assign")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Assign(
            Guid id, [FromBody] AssignRequestBody body, CancellationToken cancellationToken)
        {
            await Sender.Send(new AssignVehicleAndDriverCommand(id, body.VehicleId, body.DriverId), cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/start")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Start(Guid id, CancellationToken cancellationToken)
        {
            await Sender.Send(new StartTripCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/complete")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Complete(
            Guid id, [FromBody] CompleteTripBody body, CancellationToken cancellationToken)
        {
            await Sender.Send(new CompleteTripCommand(id, body.ActualDepartureTime, body.ActualReturnTime), cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/cancel")]
        [Authorize(Roles = "Admin,Supervisor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Cancel(
            Guid id, [FromBody] CancelRequestBody body, CancellationToken cancellationToken)
        {
            await Sender.Send(new CancelRequestCommand(id, body.Reason), cancellationToken);
            return NoContent();
        }
    }
}

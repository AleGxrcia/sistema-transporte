using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Application.Dtos.Auth;
using TransportSystem.WebApi.Contracts.Auth;

namespace TransportSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/users")]
    public class UsersController : ApiControllerBase
    {
        private readonly IAccountService _accountService;

        public UsersController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        private string CurrentUserId =>
            User.FindFirstValue("uid") ?? throw new UnauthorizedException("Usuario no autenticado.");

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<UserResult>))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _accountService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResult))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            EnsureSelfOrAdmin(id);
            var result = await _accountService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.CreateUserAsync(request, cancellationToken);
            return CreatedAtRoute("GetUserById", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResult))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            EnsureSelfOrAdmin(id);
            var result = await _accountService.UpdateUserAsync(id, request, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            await _accountService.DeleteUserAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(string id, CancellationToken cancellationToken)
        {
            await _accountService.SetUserActiveStatusAsync(id, true, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(string id, CancellationToken cancellationToken)
        {
            await _accountService.SetUserActiveStatusAsync(id, false, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ChangePassword(
            string id, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            if (id != CurrentUserId)
                throw new ForbiddenException("cambiar la contraseña de otro usuario", "el propio usuario");

            await _accountService.ChangePasswordAsync(id, request.CurrentPassword, request.NewPassword, cancellationToken);
            return NoContent();
        }

        private void EnsureSelfOrAdmin(string userId)
        {
            if (userId == CurrentUserId || User.IsInRole("Admin"))
                return;

            throw new ForbiddenException("acceder a la información de otro usuario", "Admin");
        }
    }
}

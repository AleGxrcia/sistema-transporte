using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Application.Dtos.Auth;
using TransportSystem.WebApi.Contracts.Auth;

namespace TransportSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            var result = await _accountService.LoginAsync(
                request.Email, request.Password, cancellationToken);

            return Ok(result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.RefreshTokenAsync(
                request.RefreshToken, cancellationToken);

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
        {
            await _accountService.RevokeTokenAsync(request.RefreshToken, cancellationToken);
            return NoContent();
        }

        [HttpPost("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.ConfirmEmailAsync(request.UserId, request.Token, cancellationToken);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            var origin = $"{Request.Scheme}://{Request.Host}";
            await _accountService.SendForgotPasswordEmailAsync(request.Email, origin, cancellationToken);
            return NoContent();
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            await _accountService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, cancellationToken);
            return NoContent();
        }
    }
}

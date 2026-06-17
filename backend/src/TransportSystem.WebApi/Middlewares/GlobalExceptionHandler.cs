using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Domain.Common;

namespace TransportSystem.WebApi.Middlewares
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, title, detail) = exception switch
            {
                UnauthorizedException ex =>
                    (StatusCodes.Status401Unauthorized, "Unauthorized", ex.Message),

                ForbiddenException ex =>
                    (StatusCodes.Status403Forbidden, "Forbidden", ex.Message),

                NotFoundException ex =>
                    (StatusCodes.Status404NotFound, "Not Found", ex.Message),

                ConflictException ex =>
                    (StatusCodes.Status409Conflict, "Conflict", ex.Message),

                ValidationException ex =>
                    (StatusCodes.Status422UnprocessableEntity, "Validation Error", "Se encontraron errores de validación."),

                DomainException ex =>
                    (StatusCodes.Status422UnprocessableEntity, ex.Code, ex.Message),

                OperationCanceledException =>
                    (StatusCodes.Status400BadRequest, "Cancelled", "La solicitud fue cancelada por el cliente."),

                _ =>
                    (StatusCodes.Status500InternalServerError, "Internal Server Error", "Ocurrió un error inesperado en el servidor.")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Error inesperado crítico: {Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning("Excepción controlada {StatusCode} [{Title}]: {Detail}", statusCode, title, detail);
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            if (exception is ValidationException validationException)
            {
                problemDetails.Extensions["errors"] = validationException.Errors;
            }

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
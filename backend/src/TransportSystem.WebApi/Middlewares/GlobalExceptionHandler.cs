using System.Net;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Domain.Common;
using TransportSystem.WebApi.Contracts.Common;

namespace TransportSystem.WebApi.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, code, message, errors) = exception switch
            {
                UnauthorizedException ex =>
                    (HttpStatusCode.Unauthorized, "UNAUTHORIZED", ex.Message, null),

                ForbiddenException ex =>
                    (HttpStatusCode.Forbidden, "FORBIDDEN", ex.Message, null),

                NotFoundException ex =>
                    (HttpStatusCode.NotFound, "NOT_FOUND", ex.Message, null),

                ConflictException ex =>
                    (HttpStatusCode.Conflict, "CONFLICT", ex.Message, null),

                ValidationException ex =>
                    (HttpStatusCode.UnprocessableEntity, "VALIDATION_ERROR", "Se encontraron errores de validación.", ex.Errors),

                DomainException ex =>
                    (HttpStatusCode.UnprocessableEntity, ex.Code, ex.Message, null),

                OperationCanceledException =>
                    (HttpStatusCode.BadRequest, "CANCELLED", "La solicitud fue cancelada por el cliente.", null),

                _ =>
                    (HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "Ocurrió un error inesperado en el servidor.", null)
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Error inesperado crítico: {Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning("Excepción controlada {StatusCode} [{Code}]: {Message}", (int)statusCode, code, message);
            }

            var response = new ErrorResponse(code, message, errors);

            httpContext.Response.StatusCode = (int)statusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }
}
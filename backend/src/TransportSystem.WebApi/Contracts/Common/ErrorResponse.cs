namespace TransportSystem.WebApi.Contracts.Common
{
    public record ErrorResponse(
        string Code,
        string Message,
        IDictionary<string, string[]>? Errors = null
    );
}

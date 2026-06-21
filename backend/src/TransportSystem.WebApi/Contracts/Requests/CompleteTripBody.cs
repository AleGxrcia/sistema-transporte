namespace TransportSystem.WebApi.Contracts.Requests
{
    public record CompleteTripBody(DateTime ActualDepartureTime, DateTime ActualReturnTime);
}

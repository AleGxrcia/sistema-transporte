namespace TransportSystem.WebApi.Contracts.Maintenance
{
    public record CancelScheduledMaintenanceRequest(
        string Reason
    );
}

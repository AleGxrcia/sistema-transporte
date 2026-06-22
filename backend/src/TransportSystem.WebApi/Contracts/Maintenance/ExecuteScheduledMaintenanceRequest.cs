namespace TransportSystem.WebApi.Contracts.Maintenance
{
    public record ExecuteScheduledMaintenanceRequest(
        DateTime EntryDate,
        string Workshop,
        DateTime? EstimatedExitDate,
        DateTime? NextMaintenanceDateScheduled,
        decimal? NextMaintenanceKmScheduled
    );
}

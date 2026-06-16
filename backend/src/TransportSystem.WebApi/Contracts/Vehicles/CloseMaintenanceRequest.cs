namespace TransportSystem.WebApi.Contracts.Vehicles
{
    public record CloseMaintenanceRequest(
        DateTime ActualExitDate,
        decimal Cost
    );
}

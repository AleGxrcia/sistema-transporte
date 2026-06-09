namespace TransportSystem.Core.Application.Dtos.Maintenance
{
    public record MaintenanceRecordDto(
        Guid Id,
        string Type,
        string Description,
        DateTime EntryDate,
        DateTime? EstimatedExitDate,
        DateTime? ActualExitDate,
        decimal? Cost,
        string Workshop,
        bool IsClosed
    );
}

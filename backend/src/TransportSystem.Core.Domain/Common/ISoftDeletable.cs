namespace TransportSystem.Core.Domain.Common
{
    /// <summary>
    /// Marca una entidad como archivable (soft delete). El registro nunca se elimina
    /// físicamente: se oculta de los listados operativos pero se conserva para el
    /// historial, los reportes y la auditoría.
    /// </summary>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; }
        DateTime? DeletedAt { get; }
        Guid? DeletedByUserId { get; }
    }
}

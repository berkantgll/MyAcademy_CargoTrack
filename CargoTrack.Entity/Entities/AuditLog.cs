using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities;

public class AuditLog : BaseEntity
{
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
    public string? Description { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? PerformedBy { get; set; }
}

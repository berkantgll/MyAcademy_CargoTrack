
namespace CargoTrack.DTO.AuditDtos;

public class AuditLogPageDto
{
    public List<AuditLogItemDto> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling((double)TotalCount / PageSize)
        : 0;
}

public class AuditLogItemDto
{
    public Guid Id { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public DateTime ActionDate { get; set; }

    public string? PerformedBy { get; set; }

    public string? Description { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }
}

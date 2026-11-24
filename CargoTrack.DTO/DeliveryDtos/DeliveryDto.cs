
using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.DeliveryDtos;

public class DeliveryDto
{
    public Guid CargoId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;

    public CargoStatus Status { get; set; }

    public DateTime? DeliveredAt { get; set; }
    public DateTime? VerifiedAt { get; set; }

    public string? ReceivedName { get; set; }
    public Guid? DeliveredByEmployeeId { get; set; }

    public int FailedAttemptCount { get; set; }

    public List<DeliveryExceptionDto> Exceptions { get; set; } = new();
}

public class DeliveryExceptionDto
{
    public Guid Id { get; set; }
    public int AttemptNumber { get; set; }

    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime OccurredAt { get; set; }
    public Guid? EmployeeId { get; set; }
}

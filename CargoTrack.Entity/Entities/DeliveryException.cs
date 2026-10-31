using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities;

public class DeliveryException : BaseEntity
{
    public Guid CargoId { get; set; }
    public Cargo Cargo { get; set; } = null!;
    public int AttemptNumber { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description {  get; set; } 
    public DateTime OccurredAt { get; set; }
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}

using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities;

public class Delivery : BaseEntity
{
    public Guid CargoId { get; set; }
    public Cargo Cargo { get; set; } = null!;

    public string VerificationCode { get; set; } = string.Empty;
    public DateTime? VerifiedAt { get; set; }
    public string? ReceivedName { get; set; } 

    public Guid? DeliveredByEmployeeId { get; set; }
    public Employee? DeliveredByEmployee { get; set; }

}

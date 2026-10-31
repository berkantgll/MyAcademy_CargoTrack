using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Enums;
namespace CargoTrack.Entity.Entities;

public class CargoMovement : BaseEntity
{
    public Guid CargoId { get; set; }
    public Cargo Cargo { get; set; } = null!;
    public CargoStatus? OldStatus { get; set; }
    public CargoStatus NewStatus { get; set; }
    public DateTime MovementDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? TransferCenterId { get; set; } 
    public TransferCenter? TransferCenter { get; set; }
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}

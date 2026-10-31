using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities;

public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public List<CargoMovement> ProcessedMovements { get; set; } = new();
    public List<Delivery> Deliveries { get; set; } = new();
    public List<DeliveryException> DeliveryExceptions { get; set; } = new(); 
}

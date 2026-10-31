using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities;

public class Branch : BaseEntity
{
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string BranchCity {  get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public List<Cargo> OriginCargos { get; set; } = new();
    public List<Cargo> DestinationCargos { get; set; } = new();
    public List<Employee> Employees { get; set; } = new();
    public List<CargoMovement> CargoMovements { get; set; } = new();
} 

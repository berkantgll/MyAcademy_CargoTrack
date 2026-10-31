using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities;

public class TransferCenter : BaseEntity
{
    public string CenterCode { get; set; } = string.Empty;
    public string CenterName { get; set; } = string.Empty;
    public string CenterCity { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<CargoMovement> CargoMovements { get; set; } = new();
}


using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoDtos;

public class ChangeCargoStatusDto
{
    public Guid CargoId { get; set; }
    public CargoStatus NewStatus { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? BranchId { get; set; }
    public Guid? TransferCenterId { get; set; }
    public Guid? EmployeeId { get; set; }

    public string? VerificationCode { get; set; }
    public string? ReceivedName { get; set; }
}

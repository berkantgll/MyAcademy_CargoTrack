
namespace CargoTrack.DTO.BranchDtos;

public class BranchDto
{
    public Guid Id { get; set; }

    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string BranchCity { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

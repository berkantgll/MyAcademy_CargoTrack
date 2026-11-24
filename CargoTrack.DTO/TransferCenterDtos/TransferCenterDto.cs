
namespace CargoTrack.DTO.TransferCenterDtos;

public class TransferCenterDto
{
    public Guid Id { get; set; }

    public string CenterCode { get; set; } = string.Empty;
    public string CenterName { get; set; } = string.Empty;
    public string CenterCity { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

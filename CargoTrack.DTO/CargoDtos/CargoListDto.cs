using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoDtos;

public class CargoListDto
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public CargoStatus Status { get; set; }
    public DeliveryType Type { get; set; }
    public string SenderFirstName { get; set; } =string.Empty;
    public string SenderLastName { get; set; } = string.Empty;
    public string ReceiverFirstName { get; set; } = string.Empty;
    public string ReceiverLastName { get; set; }= string.Empty;
    public string OriginBranchName { get; set; } = string.Empty;
    public string DestinationBranchName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

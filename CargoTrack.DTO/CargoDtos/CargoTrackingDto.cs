
using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoDtos;

public class CargoTrackingDto
{
    public string TrackingNumber { get; set; } = string.Empty;
    public CargoStatus Status { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public string OriginBranchName { get; set; } = string.Empty;
    public string OriginBranchCity { get; set; } = string.Empty;

    public string DestinationBranchName { get; set; } = string.Empty;
    public string DestinationBranchCity { get; set; } = string.Empty;

    public List<PublicCargoMovementDto> CargoMovements { get; set; } = new();
}

public class PublicCargoMovementDto
{
    public CargoStatus Status { get; set; }
    public DateTime MovementDate { get; set; }
}

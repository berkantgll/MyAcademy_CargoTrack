
using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoDtos;

public class UserCargoDetailDto
{
    public Guid Id { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public CargoStatus Status { get; set; }

    public string OriginBranchName { get; set; } = string.Empty;

    public string DestinationBranchName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime EstimatedDeliveryDate { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public List<UserCargoMovementDto> Movements { get; set; } = new();
}

public class UserCargoMovementDto
{
    public CargoStatus Status { get; set; }

    public DateTime MovementDate { get; set; }

    public string Description { get; set; } = string.Empty;
}


using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoDtos;

public class CargoAdminDetailDto
{
    public Guid Id { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public CargoStatus Status { get; set; }

    public string OriginBranchName { get; set; } = string.Empty;

    public string DestinationBranchName { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime EstimatedDeliveryDate { get; set; }

    public List<CargoAdminMovementDto> Movements { get; set; } = new();
}

public class CargoAdminMovementDto
{
    public CargoStatus? OldStatus { get; set; }

    public CargoStatus NewStatus { get; set; }

    public DateTime MovementDate { get; set; }

    public string Description { get; set; } = string.Empty;
}

using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoDtos;

public class CargoTrackingDto
{
    public string TrackingNumber { get; set; } = string.Empty;
    public CargoStatus Status { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

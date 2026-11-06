using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoDtos;

public class CargoMovementDto
{
    public CargoStatus? OldStatus { get; set; } 
    public CargoStatus NewStatus { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime MovementDate { get; set; }
}

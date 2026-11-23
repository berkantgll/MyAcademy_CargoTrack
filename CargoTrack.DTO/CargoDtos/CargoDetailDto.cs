using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoDtos;
public class CargoDetailDto
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public CargoStatus Status {  get; set; }
    public DeliveryType Type { get; set;  } 
    public string SenderFirstName { get; set; } = string.Empty;
    public string SenderLastName { get; set; } = string.Empty;
    public string ReceiverFirstName { get; set; } = string.Empty;
    public string ReceiverLastName { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }
    public decimal LengthCm { get; set; }
    public string OriginBranchName {  get; set; }  = string.Empty;
    public string DestinationBranchName {  get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public List<CargoMovementDto> CargoMovements { get; set; } = new(); 
    public bool IsFragile { get; set; }
    public ExtraServiceType ExtraServices { get; set; }
    public string DeliveryCity { get; set; } = string.Empty;
    public string DeliveryDistrict { get; set; } = string.Empty;
    public string DeliveryNeighborhood { get; set; } = string.Empty;
    public string DeliveryFullAddress { get; set; } = string.Empty;
    public string? DeliveryAddressDescription { get; set; }
    public decimal TotalPrice { get; set; }
}

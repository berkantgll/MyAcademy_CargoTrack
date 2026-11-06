using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoDtos;

public class UpdateCargoDto
{
    public Guid CargoId { get; set; }
    public Guid OriginBranchId { get; set; }
    public Guid DestinationBranchId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public DeliveryType Type { get; set; }
    public ExtraServiceType ExtraServices { get; set; }
    public bool IsFragile { get; set; }
    public decimal WeightKg { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }
    public decimal LengthCm { get; set; }
}

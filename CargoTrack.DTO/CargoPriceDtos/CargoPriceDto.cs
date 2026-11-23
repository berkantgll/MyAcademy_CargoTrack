using CargoTrack.Entity.Enums;

namespace CargoTrack.DTO.CargoPriceDtos;

public class CargoPriceDto
{
    public Guid Id { get; set; }

    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;

    public DeliveryType Type { get; set; }

    public decimal BasePrice { get; set; }
    public decimal PricePerUnit { get; set; }

    public decimal SpecialPackagingFee { get; set; }
    public decimal GiftWrappingFee { get; set; }
    public decimal InsuranceFee { get; set; }

    public bool IsActive { get; set; } = true;
}

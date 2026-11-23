using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Enums;

namespace CargoTrack.Entity.Entities;

public class Cargo :BaseEntity
{
    public DeliveryType Type { get; set; }
    public bool IsFragile { get; set; }
    public ExtraServiceType ExtraServices { get; set; }
    public CargoStatus Status { get; set; } = CargoStatus.Created;
    public string TrackingNumber { get; set; } = string.Empty;
    public DateTime? DeliveredAt { get; set; }

    public Guid SenderId { get; set; }
    public Customer Sender { get; set; } = null!;
    public Guid ReceiverId  { get; set; }
    public Customer Receiver { get; set; } = null!;


    public  DateTime EstimatedDeliveryDate {  get; set; }
    public decimal WeightKg { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }
    public decimal LengthCm { get; set; }

    public Guid OriginBranchId { get; set; }
    public Branch OriginBranch { get; set; } = null!;
    public Guid DestinationBranchId { get; set; }
    public Branch DestinationBranch { get; set; } = null!;

    public Guid DeliveryAddressId { get; set; }
    public Address DeliveryAddress { get; set; } = null!;

    public List<CargoMovement> CargoMovements { get; set; } = new();
    public List<DeliveryException> DeliveryExceptions { get; set; } = new();

    public Delivery? Delivery { get; set; } 

}

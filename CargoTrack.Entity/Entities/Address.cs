using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities;

public class Address : BaseEntity
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string? AddressDescription { get; set; } 
}

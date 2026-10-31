using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public List<Cargo> SentCargos { get; set; } = new();
    public List<Cargo> ReceivedCargos { get; set; } = new();
}

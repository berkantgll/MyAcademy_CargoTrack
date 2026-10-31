using Microsoft.AspNetCore.Identity;

namespace CargoTrack.Entity.Entities;

public class AppUser : IdentityUser<Guid>
{
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
}

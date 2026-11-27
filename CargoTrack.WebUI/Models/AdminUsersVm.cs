
using CargoTrack.DTO.BranchDtos;

namespace CargoTrack.WebUI.Models;

public class AdminUsersVm
{
    public List<UserRoleRowVm> Users { get; set; } = new();
    public List<BranchDto> Branches { get; set; } = new();
}

public class UserRoleRowVm
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid? BranchId { get; set; }
    public Guid? CustomerId { get; set; }
}

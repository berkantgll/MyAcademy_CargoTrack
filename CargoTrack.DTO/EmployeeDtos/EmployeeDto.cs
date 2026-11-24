
namespace CargoTrack.DTO.EmployeeDtos;

public class EmployeeDto
{
    public Guid Id { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

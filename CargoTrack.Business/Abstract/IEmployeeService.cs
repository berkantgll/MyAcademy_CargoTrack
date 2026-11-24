
using CargoTrack.DTO.EmployeeDtos;

namespace CargoTrack.Business.Abstract;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<Guid> SaveAsync(EmployeeDto dto);
    Task<bool> DeleteAsync(Guid id);
}

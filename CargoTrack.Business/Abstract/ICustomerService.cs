
using CargoTrack.DTO.CustomerDtos;

namespace CargoTrack.Business.Abstract;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<Guid> SaveAsync(CustomerDto dto);
    Task<bool> DeleteAsync(Guid id);
}

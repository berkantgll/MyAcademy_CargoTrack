
using CargoTrack.DTO.AddressDtos;

namespace CargoTrack.Business.Abstract;

public interface IAddressService
{
    Task<List<AddressDto>> GetAllAsync();
    Task<AddressDto?> GetByIdAsync(Guid id);
    Task<Guid> SaveAsync(AddressDto dto);
    Task<bool> DeleteAsync(Guid id);
}

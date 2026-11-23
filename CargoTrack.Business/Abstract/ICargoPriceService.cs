using CargoTrack.DTO.CargoPriceDtos;

namespace CargoTrack.Business.Abstract;

public interface ICargoPriceService
{
    Task<List<CargoPriceDto>> GetAllAsync();
    Task<CargoPriceDto?> GetByIdAsync(Guid id);
    Task<Guid> SaveAsync(CargoPriceDto dto);
    Task<bool> DeleteAsync(Guid id);
}

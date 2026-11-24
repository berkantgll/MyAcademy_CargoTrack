
using CargoTrack.DTO.TransferCenterDtos;

namespace CargoTrack.Business.Abstract;

public interface ITransferCenterService
{
    Task<List<TransferCenterDto>> GetAllAsync();
    Task<TransferCenterDto?> GetByIdAsync(Guid id);
    Task<Guid> SaveAsync(TransferCenterDto dto);
    Task<bool> DeleteAsync(Guid id);
}

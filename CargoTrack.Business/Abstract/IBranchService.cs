using CargoTrack.DTO.BranchDtos;

namespace CargoTrack.Business.Abstract;

public interface IBranchService
{
    Task<List<BranchDto>> GetAllAsync();
    Task<BranchDto?> GetByIdAsync(Guid id);
    Task<Guid> SaveAsync(BranchDto dto);
    Task<bool> DeleteAsync(Guid id);
}

using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.BranchDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class BranchService : IBranchService
{
    private readonly IRepository<Branch> _repository;

    public BranchService(IRepository<Branch> repository)
    {
        _repository = repository;
    }

    // Bütün şubeleri listele
    public async Task<List<BranchDto>> GetAllAsync()
    {
        var branches = await _repository.GetAllAsync();

        return branches.Adapt<List<BranchDto>>();
    }

    // ID ile şube bul
    public async Task<BranchDto?> GetByIdAsync(Guid id)
    {
        var branch = await _repository.GetByIdAsync(id);

        return branch?.Adapt<BranchDto>();
    }

    // Yeni şube ekle veya mevcut şubeyi güncelle
    public async Task<Guid> SaveAsync(BranchDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.BranchCode) ||
            string.IsNullOrWhiteSpace(dto.BranchName) ||
            string.IsNullOrWhiteSpace(dto.BranchCity))
        {
            throw new ArgumentException(
                "Şube kodu, adı ve şehri boş olamaz.");
        }

        string code = dto.BranchCode.Trim().ToUpperInvariant();

        var branches = await _repository.GetAllAsync();

        bool duplicate = branches.Any(x =>
            x.Id != dto.Id &&
            x.BranchCode.ToUpperInvariant() == code);

        if (duplicate)
        {
            throw new InvalidOperationException(
                "Bu şube kodu zaten kullanılıyor.");
        }

        Branch branch;

        if (dto.Id == Guid.Empty)
        {
            branch = new Branch
            {
                CreatedDate = DateTime.UtcNow
            };

            _repository.Add(branch);
        }
        else
        {
            branch = await _repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException(
                    "Şube bulunamadı.");

            branch.UpdatedDate = DateTime.UtcNow;
        }

        branch.BranchCode = code;
        branch.BranchName = dto.BranchName.Trim();
        branch.BranchCity = dto.BranchCity.Trim();
        branch.PhoneNumber = dto.PhoneNumber?.Trim() ?? string.Empty;
        branch.IsActive = dto.IsActive;

        await _repository.SaveChangesAsync();

        return branch.Id;
    }

    // Şubeyi soft delete ile sil
    public async Task<bool> DeleteAsync(Guid id)
    {
        var branch = await _repository.GetByIdAsync(id);

        if (branch == null)
            return false;

        branch.UpdatedDate = DateTime.UtcNow;

        _repository.Delete(branch);

        await _repository.SaveChangesAsync();

        return true;
    }
}

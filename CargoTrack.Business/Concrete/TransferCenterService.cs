
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.TransferCenterDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class TransferCenterService : ITransferCenterService
{
    private readonly IRepository<TransferCenter> _repository;

    public TransferCenterService(
        IRepository<TransferCenter> repository)
    {
        _repository = repository;
    }

    // Bütün transfer merkezlerini getir
    public async Task<List<TransferCenterDto>> GetAllAsync()
    {
        var centers = await _repository.GetAllAsync();

        return centers.Adapt<List<TransferCenterDto>>();
    }

    // ID ile transfer merkezi bul
    public async Task<TransferCenterDto?> GetByIdAsync(Guid id)
    {
        var center = await _repository.GetByIdAsync(id);

        if (center == null)
            return null;

        return center.Adapt<TransferCenterDto>();
    }

    // Transfer merkezi ekle veya güncelle
    public async Task<Guid> SaveAsync(TransferCenterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CenterCode) ||
            string.IsNullOrWhiteSpace(dto.CenterName) ||
            string.IsNullOrWhiteSpace(dto.CenterCity))
        {
            throw new ArgumentException(
                "Merkez kodu, adı ve şehri zorunludur.");
        }

        string code = dto.CenterCode.Trim().ToUpperInvariant();

        var centers = await _repository.GetAllAsync();

        bool duplicate = centers.Any(x =>
            x.Id != dto.Id &&
            x.CenterCode.ToUpperInvariant() == code);

        if (duplicate)
        {
            throw new InvalidOperationException(
                "Bu transfer merkezi kodu zaten kullanılıyor.");
        }

        TransferCenter center;

        if (dto.Id == Guid.Empty)
        {
            // Yeni transfer merkezi
            center = new TransferCenter
            {
                CreatedDate = DateTime.UtcNow
            };

            _repository.Add(center);
        }
        else
        {
            // Mevcut merkezi güncelle
            center = await _repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException(
                    "Transfer merkezi bulunamadı.");

            center.UpdatedDate = DateTime.UtcNow;
        }

        center.CenterCode = code;
        center.CenterName = dto.CenterName.Trim();
        center.CenterCity = dto.CenterCity.Trim();
        center.PhoneNumber = dto.PhoneNumber?.Trim() ?? string.Empty;
        center.IsActive = dto.IsActive;

        await _repository.SaveChangesAsync();

        return center.Id;
    }

    // Transfer merkezini soft delete ile sil
    public async Task<bool> DeleteAsync(Guid id)
    {
        var center = await _repository.GetByIdAsync(id);

        if (center == null)
            return false;

        center.IsActive = false;
        center.UpdatedDate = DateTime.UtcNow;

        _repository.Delete(center);

        await _repository.SaveChangesAsync();

        return true;
    }
}


using System.Text.Json;
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.BranchDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class BranchService : IBranchService
{
    private readonly IRepository<Branch> _repository;
    private readonly IBranchUsageRepository _usageRepository;
    private readonly IRepository<AuditLog> _auditRepository;
    private readonly ICurrentUserContext _currentUser;

    public BranchService(
        IRepository<Branch> repository,
        IBranchUsageRepository usageRepository,
        IRepository<AuditLog> auditRepository,
        ICurrentUserContext currentUser)
    {
        _repository = repository;
        _usageRepository = usageRepository;
        _auditRepository = auditRepository;
        _currentUser = currentUser;
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
                "Şube kodu, adı ve şehri zorunludur.");
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

        bool isNew = dto.Id == Guid.Empty;

        Branch branch;
        string? oldValues = null;

        if (isNew)
        {
            branch = new Branch
            {
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _repository.Add(branch);
        }
        else
        {
            branch = await _repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException(
                    "Şube bulunamadı.");

            oldValues = Snapshot(branch);

            // Aktif şubeyi pasife almadan önce
            // operasyon ve personel bağlantılarını kontrol et.
            if (branch.IsActive && !dto.IsActive)
            {
                if (await _usageRepository.HasOpenCargosAsync(branch.Id))
                {
                    throw new InvalidOperationException(
                        "Açık kargoları olan şube pasife alınamaz.");
                }

                if (await _usageRepository.HasActiveAssignmentsAsync(branch.Id))
                {
                    throw new InvalidOperationException(
                        "Şubeye bağlı aktif personel veya kullanıcı var. " +
                        "Önce ilgili atamaları değiştirin.");
                }
            }

            branch.UpdatedDate = DateTime.UtcNow;
        }

        branch.BranchCode = code;
        branch.BranchName = dto.BranchName.Trim();
        branch.BranchCity = dto.BranchCity.Trim();
        branch.PhoneNumber =
            dto.PhoneNumber?.Trim() ?? string.Empty;

        // Yeni şubeler her zaman aktif oluşturulur.
        // Mevcut şubeler düzenleme formundan pasife alınabilir.
        branch.IsActive = isNew || dto.IsActive;

        AddAudit(
            branch,
            isNew ? "Create" : "Update",
            oldValues);

        await _repository.SaveChangesAsync();

        return branch.Id;
    }

    // Şubeyi soft delete ile sil
    public async Task<bool> DeleteAsync(Guid id)
    {
        var branch = await _repository.GetByIdAsync(id);

        if (branch == null)
            return false;

        if (branch.IsActive)
        {
            throw new InvalidOperationException(
                "Şubeyi silmeden önce pasife alın.");
        }

        bool hasReferences =
            await _usageRepository.HasAnyReferencesAsync(id);

        if (hasReferences)
        {
            throw new InvalidOperationException(
                "Bu şubeye bağlı kargo, hareket, çalışan " +
                "veya kullanıcı kayıtları var. " +
                "Geçmişi korumak için şube silinemez.");
        }

        string oldValues = Snapshot(branch);

        branch.UpdatedDate = DateTime.UtcNow;

        // Mevcut GenericRepository.Delete metodu
        // IsDeleted = true yapıyor.
        _repository.Delete(branch);

        AddAudit(branch, "Delete", oldValues);

        await _repository.SaveChangesAsync();

        return true;
    }

    private static string Snapshot(Branch branch)
    {
        return JsonSerializer.Serialize(new
        {
            branch.BranchCode,
            branch.BranchName,
            branch.BranchCity,
            branch.PhoneNumber,
            branch.IsActive,
            branch.IsDeleted
        });
    }

    private void AddAudit(
        Branch branch,
        string actionType,
        string? oldValues)
    {
        var now = DateTime.UtcNow;

        var audit = new AuditLog
        {
            CreatedDate = now,
            ActionDate = now,
            EntityName = nameof(Branch),
            EntityId = branch.Id,
            ActionType = actionType,
            PerformedBy = _currentUser.UserName,
            Description =
                $"{branch.BranchName} şubesi için {actionType} işlemi.",
            OldValues = oldValues,
            NewValues = Snapshot(branch)
        };

        _auditRepository.Add(audit);
    }
}

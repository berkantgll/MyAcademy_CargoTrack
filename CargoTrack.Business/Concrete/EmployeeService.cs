
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.EmployeeDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<Employee> _repository;
    private readonly IRepository<Branch> _branchRepository;

    public EmployeeService(
        IRepository<Employee> repository,
        IRepository<Branch> branchRepository)
    {
        _repository = repository;
        _branchRepository = branchRepository;
    }

    // Çalışanları listele
    public async Task<List<EmployeeDto>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();
        var branches = await _branchRepository.GetAllAsync();

        var result = employees.Adapt<List<EmployeeDto>>();

        foreach (var employee in result)
        {
            employee.BranchName = branches
                .FirstOrDefault(x => x.Id == employee.BranchId)?
                .BranchName ?? string.Empty;
        }

        return result;
    }

    // ID ile çalışan bul
    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
            return null;

        var dto = employee.Adapt<EmployeeDto>();

        var branch = await _branchRepository
            .GetByIdAsync(employee.BranchId);

        dto.BranchName = branch?.BranchName ?? string.Empty;

        return dto;
    }

    // Çalışan ekle veya güncelle
    public async Task<Guid> SaveAsync(EmployeeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EmployeeCode) ||
            string.IsNullOrWhiteSpace(dto.FullName) ||
            string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            throw new ArgumentException(
                "Çalışan kodu, adı ve telefonu zorunludur.");
        }

        var branch = await _branchRepository
            .GetByIdAsync(dto.BranchId);

        if (branch == null || !branch.IsActive)
        {
            throw new InvalidOperationException(
                "Geçerli ve aktif bir şube seçilmelidir.");
        }

        string code = dto.EmployeeCode.Trim().ToUpperInvariant();

        var employees = await _repository.GetAllAsync();

        if (employees.Any(x =>
            x.Id != dto.Id &&
            x.EmployeeCode.ToUpperInvariant() == code))
        {
            throw new InvalidOperationException(
                "Bu çalışan kodu zaten kullanılıyor.");
        }

        Employee employee;

        if (dto.Id == Guid.Empty)
        {
            employee = new Employee
            {
                CreatedDate = DateTime.UtcNow
            };

            _repository.Add(employee);
        }
        else
        {
            employee = await _repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException(
                    "Çalışan bulunamadı.");

            employee.UpdatedDate = DateTime.UtcNow;
        }

        employee.EmployeeCode = code;
        employee.FullName = dto.FullName.Trim();
        employee.PhoneNumber = dto.PhoneNumber.Trim();
        employee.BranchId = dto.BranchId;
        employee.IsActive = dto.IsActive;

        await _repository.SaveChangesAsync();

        return employee.Id;
    }

    // Çalışanı soft delete ile sil
    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
            return false;

        employee.IsActive = false;
        employee.UpdatedDate = DateTime.UtcNow;

        _repository.Delete(employee);

        await _repository.SaveChangesAsync();

        return true;
    }
}

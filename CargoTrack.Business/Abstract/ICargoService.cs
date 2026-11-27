using CargoTrack.DTO.CargoDtos;
using CargoTrack.Entity.Enums;

namespace CargoTrack.Business.Abstract;

public interface ICargoService
{
    Task<bool> ExistsCargoAsync(Guid cargoId);
    Task<CargoDetailDto?> GetCargoAsync(Guid cargoId);
    Task<List<CargoListDto>> GetAllCargoAsync();
    Task<Guid> CreateCargoAsync(CreateCargoDto dto);
    Task<bool> UpdateCargoAsync(UpdateCargoDto dto);
    Task<bool> DeleteCargoAsync(Guid cargoId);
    Task<bool> ChangeCargoStatusAsync(ChangeCargoStatusDto dto);
    Task<CargoTrackingDto?> TrackCargoAsync(string trackingNumber);
    Task<List<CargoListDto>> GetByCustomerIdAsync(Guid customerId);
    Task<List<CargoListDto>> GetByBranchIdAsync(Guid branchId);
    Task<CargoAdminDetailDto?> GetAdminCargoDetailsAsync(Guid cargoId);
    Task<UpdateCargoDto?> GetCargoForEditAsync(Guid cargoId);
    Task<CargoAdminDetailDto?> GetManagerCargoAsync(
    Guid branchId, Guid cargoId);
    Task<bool> ChangeCargoStatusForBranchAsync(
        Guid branchId, ChangeCargoStatusDto dto);
    Task<(List<CargoListDto> Items, int TotalCount)> SearchCargosAsync(
    string? search,
    CargoStatus? status,
    Guid? branchId,
    int page,
    int pageSize);
    Task<UserCargoDetailDto?> GetUserCargoDetailsAsync(
    Guid customerId,
    Guid cargoId);
}
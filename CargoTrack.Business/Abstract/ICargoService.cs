using CargoTrack.DTO.CargoDtos;

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
}
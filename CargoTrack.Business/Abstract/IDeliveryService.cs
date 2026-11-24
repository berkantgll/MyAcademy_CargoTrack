
using CargoTrack.DTO.DeliveryDtos;

namespace CargoTrack.Business.Abstract;

public interface IDeliveryService
{
    Task<DeliveryDto?> GetByCargoIdAsync(Guid cargoId);
}


using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.DeliveryDtos;

namespace CargoTrack.Business.Concrete;

public class DeliveryService : IDeliveryService
{
    private readonly ICargoRepository _cargoRepository;

    public DeliveryService(ICargoRepository cargoRepository)
    {
        _cargoRepository = cargoRepository;
    }

    // Kargoya ait teslimat bilgilerini getir
    public async Task<DeliveryDto?> GetByCargoIdAsync(Guid cargoId)
    {
        var cargo = await _cargoRepository
            .GetByIdWithDetailsAsync(cargoId);

        if (cargo == null)
            return null;

        var dto = new DeliveryDto
        {
            CargoId = cargo.Id,
            TrackingNumber = cargo.TrackingNumber,
            Status = cargo.Status,
            DeliveredAt = cargo.DeliveredAt,

            VerifiedAt = cargo.Delivery?.VerifiedAt,
            ReceivedName = cargo.Delivery?.ReceivedName,

            DeliveredByEmployeeId =
                cargo.Delivery?.DeliveredByEmployeeId,

            FailedAttemptCount = cargo.DeliveryExceptions.Count,

            Exceptions = cargo.DeliveryExceptions
                .OrderByDescending(x => x.OccurredAt)
                .Select(x => new DeliveryExceptionDto
                {
                    Id = x.Id,
                    AttemptNumber = x.AttemptNumber,
                    Reason = x.Reason,
                    Description = x.Description,
                    OccurredAt = x.OccurredAt,
                    EmployeeId = x.EmployeeId
                })
                .ToList()
        };

        return dto;
    }
}

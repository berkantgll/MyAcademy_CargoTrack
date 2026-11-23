
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.CargoDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;
using Mapster;
using System.Security.Cryptography;

namespace CargoTrack.Business.Concrete;

public class CargoService : ICargoService
{
    private readonly ICargoRepository _cargoRepository;

    public CargoService(ICargoRepository cargoRepository)
    {
        _cargoRepository = cargoRepository;
    }

    // Hangi kargo durumundan hangisine geçilebilir?
    private bool IsAllowedTransition(CargoStatus current, CargoStatus next)
    {
        return (current, next) switch
        {
            (CargoStatus.Created, CargoStatus.AtOriginBranch) => true,

            (CargoStatus.AtOriginBranch, CargoStatus.AtTransferCenter) => true,

            (CargoStatus.AtTransferCenter, CargoStatus.AtDestinationBranch) => true,

            (CargoStatus.AtDestinationBranch, CargoStatus.OutForDelivery) => true,

            (CargoStatus.OutForDelivery, CargoStatus.Delivered) => true,

            (CargoStatus.OutForDelivery, CargoStatus.DeliveryFailed) => true,

            (CargoStatus.DeliveryFailed, CargoStatus.OutForDelivery) => true,

            (CargoStatus.ReturnInProgress, CargoStatus.ReturnedToSender) => true,

            _ => false
        };
    }

    // 3 başarısız teslimat oldu mu?
    private bool ShouldStartReturn(List<CargoMovement> movements)
    {
        int failedCount = movements.Count(
            x => x.NewStatus == CargoStatus.DeliveryFailed);

        return failedCount >= 3;
    }

    // Kargo durumunu değiştir
    public async Task<bool> ChangeCargoStatusAsync(ChangeCargoStatusDto dto)
    {
        var cargo = await _cargoRepository
            .GetByIdWithDetailsAsync(dto.CargoId);

        if (cargo == null)
            return false;

        if (!IsAllowedTransition(cargo.Status, dto.NewStatus))
            return false;

        // Teslimat için kod, çalışan ve teslim alan kişi kontrolü
        if (dto.NewStatus == CargoStatus.Delivered)
        {
            if (cargo.Delivery == null ||
                string.IsNullOrWhiteSpace(dto.VerificationCode) ||
                cargo.Delivery.VerificationCode != dto.VerificationCode.Trim() ||
                string.IsNullOrWhiteSpace(dto.ReceivedName) ||
                !dto.EmployeeId.HasValue)
            {
                return false;
            }
        }

        var now = DateTime.UtcNow;
        var oldStatus = cargo.Status;

        cargo.Status = dto.NewStatus;
        cargo.UpdatedDate = now;

        string description = string.IsNullOrWhiteSpace(dto.Description)
            ? dto.NewStatus.ToString()
            : dto.Description.Trim();

        // Durum değişikliğini hareket geçmişine ekle
        cargo.CargoMovements.Add(new CargoMovement
        {
            CargoId = cargo.Id,
            OldStatus = oldStatus,
            NewStatus = dto.NewStatus,
            MovementDate = now,
            CreatedDate = now,
            Description = description,
            BranchId = dto.BranchId,
            TransferCenterId = dto.TransferCenterId,
            EmployeeId = dto.EmployeeId
        });

        // Dağıtıma çıktığında teslimat kodu oluştur
        if (dto.NewStatus == CargoStatus.OutForDelivery)
        {
            string code = RandomNumberGenerator
                .GetInt32(0, 1000000)
                .ToString("D6");

            if (cargo.Delivery == null)
            {
                cargo.Delivery = new Delivery
                {
                    CargoId = cargo.Id,
                    VerificationCode = code,
                    CreatedDate = now
                };
            }
            else
            {
                cargo.Delivery.VerificationCode = code;
                cargo.Delivery.VerifiedAt = null;
                cargo.Delivery.ReceivedName = null;
                cargo.Delivery.DeliveredByEmployeeId = null;
                cargo.Delivery.UpdatedDate = now;
            }
        }

        // Doğrulama kodu doğruysa teslimatı tamamla
        if (dto.NewStatus == CargoStatus.Delivered)
        {
            cargo.DeliveredAt = now;

            cargo.Delivery!.VerifiedAt = now;
            cargo.Delivery.ReceivedName = dto.ReceivedName!.Trim();
            cargo.Delivery.DeliveredByEmployeeId = dto.EmployeeId;
            cargo.Delivery.VerificationCode = string.Empty;
            cargo.Delivery.UpdatedDate = now;
        }

        // Teslimat başarısızsa kayıt oluştur
        if (dto.NewStatus == CargoStatus.DeliveryFailed)
        {
            int attemptCount = cargo.CargoMovements.Count(
                x => x.NewStatus == CargoStatus.DeliveryFailed);

            cargo.DeliveryExceptions.Add(new DeliveryException
            {
                CargoId = cargo.Id,
                AttemptNumber = attemptCount,
                Reason = description,
                OccurredAt = now,
                CreatedDate = now,
                EmployeeId = dto.EmployeeId
            });

            // Üçüncü başarısız denemede iade başlat
            if (ShouldStartReturn(cargo.CargoMovements))
            {
                cargo.Status = CargoStatus.ReturnInProgress;

                if (cargo.Delivery != null)
                {
                    cargo.Delivery.VerificationCode = string.Empty;
                    cargo.Delivery.UpdatedDate = now;
                }

                cargo.CargoMovements.Add(new CargoMovement
                {
                    CargoId = cargo.Id,
                    OldStatus = CargoStatus.DeliveryFailed,
                    NewStatus = CargoStatus.ReturnInProgress,
                    MovementDate = now,
                    CreatedDate = now,
                    Description = "3 başarısız teslimat sonrası iade başlatıldı.",
                    BranchId = dto.BranchId,
                    EmployeeId = dto.EmployeeId
                });
            }
        }

        await _cargoRepository.SaveChangesAsync();

        return true;
    }

    // Yeni kargo oluştur
    public async Task<Guid> CreateCargoAsync(CreateCargoDto dto)
    {
        var now = DateTime.UtcNow;

        var cargo = new Cargo
        {
            SenderId = dto.SenderId,
            ReceiverId = dto.ReceiverId,
            OriginBranchId = dto.OriginBranchId,
            DestinationBranchId = dto.DestinationBranchId,
            DeliveryAddressId = dto.DeliveryAddressId,

            Type = dto.Type,
            ExtraServices = dto.ExtraServices,
            IsFragile = dto.IsFragile,

            WeightKg = dto.WeightKg,
            WidthCm = dto.WidthCm,
            HeightCm = dto.HeightCm,
            LengthCm = dto.LengthCm,

            EstimatedDeliveryDate = dto.EstimatedDeliveryDate,
            Status = CargoStatus.Created,
            CreatedDate = now
        };

        // Takip numarası üret
        int number = RandomNumberGenerator.GetInt32(100000, 1000000);

        cargo.TrackingNumber = $"MYC-{now.Year}-{number}";

        // İlk hareket kaydını ekle
        cargo.CargoMovements.Add(new CargoMovement
        {
            CargoId = cargo.Id,
            OldStatus = null,
            NewStatus = CargoStatus.Created,
            MovementDate = now,
            CreatedDate = now,
            Description = "Kargo kaydı oluşturuldu."
        });

        _cargoRepository.Add(cargo);

        await _cargoRepository.SaveChangesAsync();

        return cargo.Id;
    }

    // Kargo güncelle
    public async Task<bool> UpdateCargoAsync(UpdateCargoDto dto)
    {
        var cargo = await _cargoRepository.GetByIdAsync(dto.CargoId);

        if (cargo == null)
            return false;

        cargo.WeightKg = dto.WeightKg;
        cargo.WidthCm = dto.WidthCm;
        cargo.HeightCm = dto.HeightCm;
        cargo.LengthCm = dto.LengthCm;

        cargo.OriginBranchId = dto.OriginBranchId;
        cargo.DestinationBranchId = dto.DestinationBranchId;
        cargo.DeliveryAddressId = dto.DeliveryAddressId;

        cargo.Type = dto.Type;
        cargo.ExtraServices = dto.ExtraServices;
        cargo.IsFragile = dto.IsFragile;
        cargo.UpdatedDate = DateTime.UtcNow;

        await _cargoRepository.SaveChangesAsync();

        return true;
    }

    // Kargo sil (Soft Delete)
    public async Task<bool> DeleteCargoAsync(Guid cargoId)
    {
        var cargo = await _cargoRepository.GetByIdAsync(cargoId);

        if (cargo == null)
            return false;

        cargo.UpdatedDate = DateTime.UtcNow;

        _cargoRepository.Delete(cargo);

        await _cargoRepository.SaveChangesAsync();

        return true;
    }

    // Kargo var mı?
    public async Task<bool> ExistsCargoAsync(Guid cargoId)
    {
        return await _cargoRepository.ExistsAsync(cargoId);
    }

    // Bütün kargoları getir
    public async Task<List<CargoListDto>> GetAllCargoAsync()
    {
        var cargos = await _cargoRepository.GetAllWithDetailsAsync();

        return cargos.Adapt<List<CargoListDto>>();
    }

    // Tek bir kargonun detaylarını getir
    public async Task<CargoDetailDto?> GetCargoAsync(Guid cargoId)
    {
        var cargo = await _cargoRepository
            .GetByIdWithDetailsAsync(cargoId);

        if (cargo == null)
            return null;

        return cargo.Adapt<CargoDetailDto>();
    }

    // Takip numarasıyla kargo sorgula
    public async Task<CargoTrackingDto?> TrackCargoAsync(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            return null;

        trackingNumber = trackingNumber.Trim().ToUpperInvariant();

        var cargo = await _cargoRepository
            .GetByTrackingNumberAsync(trackingNumber);

        if (cargo == null)
            return null;

        var dto = cargo.Adapt<CargoTrackingDto>();

        dto.OriginBranchName = cargo.OriginBranch.BranchName;
        dto.OriginBranchCity = cargo.OriginBranch.BranchCity;

        dto.DestinationBranchName = cargo.DestinationBranch.BranchName;
        dto.DestinationBranchCity = cargo.DestinationBranch.BranchCity;

        dto.CargoMovements = cargo.CargoMovements
            .OrderByDescending(x => x.MovementDate)
            .Select(x => new PublicCargoMovementDto
            {
                Status = x.NewStatus,
                MovementDate = x.MovementDate
            })
            .ToList();

        return dto;
    }
}

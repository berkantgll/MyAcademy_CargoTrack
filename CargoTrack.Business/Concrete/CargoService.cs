using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.CargoDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;
using Mapster;
using System.Security.Cryptography;
using FluentValidation;
using System.Text.Json;

namespace CargoTrack.Business.Concrete;

public class CargoService : ICargoService
{
    private readonly ICargoRepository _cargoRepository;
    private readonly ICargoPricingService _pricingService;
    private readonly IRepository<Branch> _branchRepository;
    private readonly IRepository<AuditLog> _auditRepository;

    private readonly IValidator<CreateCargoDto> _createValidator;
    private readonly IValidator<UpdateCargoDto> _updateValidator;

    public CargoService(
        ICargoRepository cargoRepository,
        ICargoPricingService pricingService,
        IRepository<Branch> branchRepository,
        IRepository<AuditLog> auditRepository,
        IValidator<CreateCargoDto> createValidator,
        IValidator<UpdateCargoDto> updateValidator)
    {
        _cargoRepository = cargoRepository;
        _pricingService = pricingService;
        _branchRepository = branchRepository;
        _auditRepository = auditRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private void AddAudit(
        Cargo cargo,
        string actionType,
        string description,
        string? oldValues = null,
        string? newValues = null)
    {
        var now = DateTime.UtcNow;

        var log = new AuditLog
        {
            EntityName = nameof(Cargo),
            EntityId = cargo.Id,
            ActionType = actionType,
            ActionDate = now,
            CreatedDate = now,
            Description = description,
            OldValues = oldValues,
            NewValues = newValues
        };

        _auditRepository.Add(log);
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

        AddAudit(
               cargo,
              "StatusChanged",
               description,
               oldStatus.ToString(),
               cargo.Status.ToString());

        await _cargoRepository.SaveChangesAsync();

        return true;
    }

    // Yeni kargo oluştur

    public async Task<Guid> CreateCargoAsync(CreateCargoDto dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);
        var now = DateTime.UtcNow;

        var origin = await _branchRepository.GetByIdAsync(dto.OriginBranchId);
        var destination = await _branchRepository.GetByIdAsync(dto.DestinationBranchId);

        if (origin == null || destination == null ||
            !origin.IsActive || !destination.IsActive)
        {
            throw new InvalidOperationException(
                "Geçerli ve aktif şubeler seçilmelidir.");
        }

        bool sameCity = string.Equals(
            origin.BranchCity.Trim(),
            destination.BranchCity.Trim(),
            StringComparison.OrdinalIgnoreCase);

        // Şimdilik örnek SLA süreleri
        int deliveryDays = dto.Type == DeliveryType.Urgent
            ? 1
            : sameCity ? 2 : 4;

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
            EstimatedDeliveryDate = now.Date.AddDays(deliveryDays),
            Status = CargoStatus.Created,
            CreatedDate = now
        };

        // Fiyatı otomatik hesapla
        cargo.TotalPrice = await _pricingService
            .CalculatePriceAsync(cargo);

        // Takip numarası üret
        const int maxAttempts = 10;
        bool uniqueNumberFound = false;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int number = RandomNumberGenerator.GetInt32(100000, 1000000);
            string trackingNumber = $"MYC-{now.Year}-{number}";

            var existing = await _cargoRepository
                .GetByTrackingNumberAsync(trackingNumber);

            if (existing == null)
            {
                cargo.TrackingNumber = trackingNumber;
                uniqueNumberFound = true;
                break;
            }
        }

        if (!uniqueNumberFound)
        {
            throw new InvalidOperationException(
                "Benzersiz takip numarası üretilemedi.");
        }

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

        AddAudit(
    cargo,
    "Created",
    $"Kargo oluşturuldu: {cargo.TrackingNumber}",
    null,
    cargo.Status.ToString());

        await _cargoRepository.SaveChangesAsync();

        return cargo.Id;
    }


    // Kargo güncelle

    public async Task<bool> UpdateCargoAsync(UpdateCargoDto dto)
    {
        await _updateValidator.ValidateAndThrowAsync(dto);
        var cargo = await _cargoRepository.GetByIdAsync(dto.CargoId);

        if (cargo == null)
            return false;

        var oldValues = JsonSerializer.Serialize(new
        {
            cargo.WeightKg,
            cargo.WidthCm,
            cargo.HeightCm,
            cargo.LengthCm,
            cargo.TotalPrice
        });

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

        // Değişen bilgilere göre fiyatı tekrar hesapla
        cargo.TotalPrice = await _pricingService.CalculatePriceAsync(cargo);

        cargo.UpdatedDate = DateTime.UtcNow;

        var newValues = JsonSerializer.Serialize(new
        {
            cargo.WeightKg,
            cargo.WidthCm,
            cargo.HeightCm,
            cargo.LengthCm,
            cargo.TotalPrice 
        });

        AddAudit(
            cargo,
            "Updated",
            "Kargo bilgileri güncellendi.",
            oldValues,
            newValues);

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

        AddAudit(
    cargo,
    "Deleted",
    "Kargo soft delete ile silindi.",
    "IsDeleted = false",
    "IsDeleted = true");

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

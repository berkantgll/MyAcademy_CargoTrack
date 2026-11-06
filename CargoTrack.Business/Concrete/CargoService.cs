using CargoTrack.Business.Abstract;
using CargoTrack.DTO.CargoDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;
using System.Security.Cryptography;

namespace CargoTrack.Business.Concrete;

public class CargoService : ICargoService
{

    private readonly IRepository<Cargo> _cargoRepository;

    public CargoService(IRepository<Cargo> cargoRepository)
    {
        _cargoRepository = cargoRepository;
    }

    private bool IsAllowedTransition(CargoStatus current, CargoStatus next)
    {
        return (current, next) switch
        {
            (CargoStatus.Created, CargoStatus.AtOriginBranch) => true,
            (CargoStatus.AtOriginBranch, CargoStatus.AtTransferCenter) => true,
            (CargoStatus.AtTransferCenter, CargoStatus.AtDestinationBranch) => true,
            (CargoStatus.AtDestinationBranch, CargoStatus.OutForDelivery) => true,
            (CargoStatus.OutForDelivery, CargoStatus.DeliveryFailed) => true,
            (CargoStatus.DeliveryFailed, CargoStatus.OutForDelivery) => true,
            (CargoStatus.ReturnInProgress,CargoStatus.ReturnedToSender)=>true,
            _ => false
        };
    }

    private bool ShouldStartReturn(List<CargoMovement> movements)
    {
        int failedCount = movements.Count(x => x.NewStatus == CargoStatus.DeliveryFailed);
        
        if(failedCount >= 3)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> ChangeCargoStatusAsync(ChangeCargoStatusDto dto)
    {
        var cargo = await _cargoRepository.GetByIdAsync(dto.CargoId);
      
        if (cargo == null)
        {
            return false;
        }

        if (cargo.Status == dto.NewStatus)
        {
            return false;
        }

        if (!IsAllowedTransition(cargo.Status, dto.NewStatus))
        {
            return false;
        }

        var oldStatus = cargo.Status;
        cargo.Status = dto.NewStatus;

        var movement = new CargoMovement
        {
            CargoId = cargo.Id,
            OldStatus = oldStatus,
            NewStatus = dto.NewStatus,
            MovementDate = DateTime.UtcNow,
            Description = dto.Description,
            BranchId = dto.BranchId,
            TransferCenterId = dto.TransferCenterId,
            EmployeeId = dto.EmployeeId
        };

        cargo.CargoMovements.Add(movement);

        await _cargoRepository.SaveChangesAsync();

        return true;
    }

    public async Task<Guid> CreateCargoAsync(CreateCargoDto dto)
    {
        Cargo cargo = new Cargo();

        cargo.SenderId = dto.SenderId;
        cargo.ReceiverId = dto.ReceiverId;
        cargo.OriginBranchId = dto.OriginBranchId;
        cargo.DestinationBranchId = dto.DestinationBranchId;
        cargo.DeliveryAddressId = dto.DeliveryAddressId;
        cargo.Type = dto.Type;
        cargo.ExtraServices = dto.ExtraServices;
        cargo.IsFragile = dto.IsFragile;
        cargo.WeightKg = dto.WeightKg;
        cargo.WidthCm = dto.WidthCm;
        cargo.HeightCm = dto.HeightCm;
        cargo.LengthCm = dto.LengthCm;
        cargo.EstimatedDeliveryDate = dto.EstimatedDeliveryDate;

        cargo.Status = CargoStatus.Created;

        int number = RandomNumberGenerator.GetInt32(100000, 1000000);
        string trackingNumber = $"MYC-{DateTime.UtcNow.Year}-{number}";
        cargo.TrackingNumber = trackingNumber;

        var movement = new CargoMovement
        {
            CargoId = cargo.Id,
            OldStatus = null,
            NewStatus = CargoStatus.Created,
            MovementDate = DateTime.UtcNow,
            Description = "Kargo Kaydı Oluşturuldu.",
        };

        cargo.CargoMovements.Add(movement);

        _cargoRepository.Add(cargo);

        await _cargoRepository.SaveChangesAsync();

        return cargo.Id;

    }

    public async Task<bool> DeleteCargoAsync(Guid cargoId)
    {
       var cargo = await _cargoRepository.GetByIdAsync(cargoId);

        if (cargo == null)
        {
            return false;
        }

        _cargoRepository.Delete(cargo);

       await _cargoRepository.SaveChangesAsync();

        return true;
    }

    public Task<bool> ExistsCargoAsync(Guid cargoId)
    {
        throw new NotImplementedException();
    }

    public Task<List<CargoListDto>> GetAllCargoAsync()
    {
        throw new NotImplementedException();
    }

    public Task<CargoDetailDto?> GetCargoAsync(Guid cargoId)
    {
        throw new NotImplementedException();
    }

    public Task<CargoTrackingDto?> TrackCargoAsync(string trackingNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateCargoAsync(UpdateCargoDto dto)
    {
        var cargo = await _cargoRepository.GetByIdAsync(dto.CargoId);

        if(cargo == null)
        {
            return false;
        }

        cargo.WeightKg = dto.WeightKg;
        cargo.WidthCm= dto.WidthCm;
        cargo.HeightCm = dto.HeightCm;
        cargo.LengthCm = dto.LengthCm;
        cargo.OriginBranchId = dto.OriginBranchId;
        cargo.DestinationBranchId = dto.DestinationBranchId;
        cargo.DeliveryAddressId = dto.DeliveryAddressId;
        cargo.Type = dto.Type;
        cargo.ExtraServices = dto.ExtraServices;
        cargo.IsFragile = dto.IsFragile;

        await _cargoRepository.SaveChangesAsync();

        return true;
    }
}


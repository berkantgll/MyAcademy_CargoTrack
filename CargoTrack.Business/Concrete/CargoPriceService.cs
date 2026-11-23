
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.CargoPriceDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class CargoPriceService : ICargoPriceService
{
    private readonly IRepository<CargoPrice> _repository;

    public CargoPriceService(IRepository<CargoPrice> repository)
    {
        _repository = repository;
    }

    // Bütün fiyat tarifelerini getir
    public async Task<List<CargoPriceDto>> GetAllAsync()
    {
        var prices = await _repository.GetAllAsync();

        return prices.Adapt<List<CargoPriceDto>>();
    }

    // Tek fiyat tarifesini getir
    public async Task<CargoPriceDto?> GetByIdAsync(Guid id)
    {
        var price = await _repository.GetByIdAsync(id);

        return price?.Adapt<CargoPriceDto>();
    }

    // Yeni tarife ekle veya mevcut tarifeyi güncelle
    public async Task<Guid> SaveAsync(CargoPriceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OriginCity) ||
            string.IsNullOrWhiteSpace(dto.DestinationCity))
        {
            throw new ArgumentException(
                "Çıkış ve varış şehirleri boş olamaz.");
        }

        if (!Enum.IsDefined(typeof(DeliveryType), dto.Type))
        {
            throw new ArgumentException(
                "Geçerli gönderi tipi seçilmelidir.");
        }

        if (dto.BasePrice < 0 ||
            dto.PricePerUnit <= 0 ||
            dto.SpecialPackagingFee < 0 ||
            dto.GiftWrappingFee < 0 ||
            dto.InsuranceFee < 0)
        {
            throw new ArgumentException(
                "Fiyatlar geçerli ve negatif olmayan değerler olmalıdır.");
        }

        string origin = dto.OriginCity.Trim();
        string destination = dto.DestinationCity.Trim();

        var prices = await _repository.GetAllAsync();

        bool duplicate = prices.Any(x =>
            x.Id != dto.Id &&
            x.Type == dto.Type &&
            string.Equals(x.OriginCity, origin,
                StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.DestinationCity, destination,
                StringComparison.OrdinalIgnoreCase));

        if (duplicate)
        {
            throw new InvalidOperationException(
                "Bu güzergâh ve gönderi tipi için tarife zaten var.");
        }

        CargoPrice price;

        if (dto.Id == Guid.Empty)
        {
            price = new CargoPrice
            {
                CreatedDate = DateTime.UtcNow
            };

            _repository.Add(price);
        }
        else
        {
            price = await _repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException(
                    "Fiyat tarifesi bulunamadı.");

            price.UpdatedDate = DateTime.UtcNow;
        }

        price.OriginCity = origin;
        price.DestinationCity = destination;
        price.Type = dto.Type;
        price.BasePrice = dto.BasePrice;
        price.PricePerUnit = dto.PricePerUnit;
        price.SpecialPackagingFee = dto.SpecialPackagingFee;
        price.GiftWrappingFee = dto.GiftWrappingFee;
        price.InsuranceFee = dto.InsuranceFee;
        price.IsActive = dto.IsActive;

        await _repository.SaveChangesAsync();

        return price.Id;
    }

    // Tarifeyi sil (Soft Delete)
    public async Task<bool> DeleteAsync(Guid id)
    {
        var price = await _repository.GetByIdAsync(id);

        if (price == null)
            return false;

        price.UpdatedDate = DateTime.UtcNow;
        _repository.Delete(price);

        await _repository.SaveChangesAsync();

        return true;
    }
}

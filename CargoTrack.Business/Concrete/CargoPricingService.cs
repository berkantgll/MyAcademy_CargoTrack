
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;

namespace CargoTrack.Business.Concrete;

public class CargoPricingService : ICargoPricingService
{
    private readonly IRepository<CargoPrice> _priceRepository;
    private readonly IRepository<Branch> _branchRepository;

    public CargoPricingService(
        IRepository<CargoPrice> priceRepository,
        IRepository<Branch> branchRepository)
    {
        _priceRepository = priceRepository;
        _branchRepository = branchRepository;
    }

    public async Task<decimal> CalculatePriceAsync(Cargo cargo)
    {
        if (cargo.WeightKg <= 0 ||
            cargo.WidthCm <= 0 ||
            cargo.HeightCm <= 0 ||
            cargo.LengthCm <= 0)
        {
            throw new ArgumentException(
                "Kargo ağırlığı ve boyutları sıfırdan büyük olmalıdır.");
        }

        var origin = await _branchRepository
            .GetByIdAsync(cargo.OriginBranchId);

        var destination = await _branchRepository
            .GetByIdAsync(cargo.DestinationBranchId);

        if (origin == null || destination == null ||
            !origin.IsActive || !destination.IsActive)
        {
            throw new InvalidOperationException(
                "Geçerli ve aktif şubeler seçilmelidir.");
        }

        var prices = await _priceRepository.GetAllAsync();

        var priceRule = prices
            .Where(x => x.IsActive && x.Type == cargo.Type)
            .FirstOrDefault(x =>
                string.Equals(x.OriginCity.Trim(),
                    origin.BranchCity.Trim(),
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.DestinationCity.Trim(),
                    destination.BranchCity.Trim(),
                    StringComparison.OrdinalIgnoreCase));

        if (priceRule == null)
        {
            throw new InvalidOperationException(
                "Seçilen güzergâh ve teslimat tipi için fiyat tarifesi bulunamadı.");
        }

        // Desi hesapla
        decimal desi =
            cargo.WidthCm * cargo.LengthCm * cargo.HeightCm / 3000m;

        // Ağırlık veya desiden büyük olanı kullan
        decimal billableWeight = Math.Ceiling(
            Math.Max(cargo.WeightKg, desi));

        decimal total =
            priceRule.BasePrice +
            billableWeight * priceRule.PricePerUnit;

        // Seçilen ek hizmetlerin ücretlerini ekle
        if (cargo.ExtraServices.HasFlag(
            ExtraServiceType.SpecialPackaging))
        {
            total += priceRule.SpecialPackagingFee;
        }

        if (cargo.ExtraServices.HasFlag(
            ExtraServiceType.GiftWrapping))
        {
            total += priceRule.GiftWrappingFee;
        }

        if (cargo.ExtraServices.HasFlag(
            ExtraServiceType.Insurance))
        {
            total += priceRule.InsuranceFee;
        }

        return Math.Round(total, 2,
            MidpointRounding.AwayFromZero);
    }
}

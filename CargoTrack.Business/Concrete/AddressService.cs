
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.AddressDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Concrete;

public class AddressService : IAddressService
{
    private readonly IRepository<Address> _repository;
    private readonly IRepository<Cargo> _cargoRepository;

    public AddressService(
        IRepository<Address> repository,
        IRepository<Cargo> cargoRepository)
    {
        _repository = repository;
        _cargoRepository = cargoRepository;
    }

    // Bütün adresleri listele
    public async Task<List<AddressDto>> GetAllAsync()
    {
        var addresses = await _repository.GetAllAsync();

        return addresses.Adapt<List<AddressDto>>();
    }

    // ID ile adres bul
    public async Task<AddressDto?> GetByIdAsync(Guid id)
    {
        var address = await _repository.GetByIdAsync(id);

        if (address == null)
            return null;

        return address.Adapt<AddressDto>();
    }

    // Yeni adres ekle veya mevcut adresi güncelle
    public async Task<Guid> SaveAsync(AddressDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Country) ||
            string.IsNullOrWhiteSpace(dto.City) ||
            string.IsNullOrWhiteSpace(dto.District) ||
            string.IsNullOrWhiteSpace(dto.Neighborhood) ||
            string.IsNullOrWhiteSpace(dto.FullAddress))
        {
            throw new ArgumentException(
                "Adres bilgileri boş bırakılamaz.");
        }

        Address address;

        if (dto.Id == Guid.Empty)
        {
            address = new Address
            {
                CreatedDate = DateTime.UtcNow
            };

            _repository.Add(address);
        }
        else
        {
            address = await _repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException(
                    "Adres bulunamadı.");

            address.UpdatedDate = DateTime.UtcNow;
        }

        address.Country = dto.Country.Trim();
        address.City = dto.City.Trim();
        address.District = dto.District.Trim();
        address.Neighborhood = dto.Neighborhood.Trim();
        address.FullAddress = dto.FullAddress.Trim();

        address.AddressDescription =
            string.IsNullOrWhiteSpace(dto.AddressDescription)
                ? null
                : dto.AddressDescription.Trim();

        await _repository.SaveChangesAsync();

        return address.Id;
    }

    // Adresi soft delete ile sil
    public async Task<bool> DeleteAsync(Guid id)
    {
        var address = await _repository.GetByIdAsync(id);

        if (address == null)
            return false;

        // Aktif bir kargonun adresi silinmesin
        var cargos = await _cargoRepository.GetAllAsync();

        if (cargos.Any(x => x.DeliveryAddressId == id))
            return false;

        address.UpdatedDate = DateTime.UtcNow;

        _repository.Delete(address);

        await _repository.SaveChangesAsync();

        return true;
    }
}

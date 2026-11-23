
using CargoTrack.Business.Abstract;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DTO.CustomerDtos;
using CargoTrack.Entity.Entities;
using Mapster;
using System.Net.Mail;

namespace CargoTrack.Business.Concrete;

public class CustomerService : ICustomerService
{
    private readonly IRepository<Customer> _repository;

    public CustomerService(IRepository<Customer> repository)
    {
        _repository = repository;
    }

    // Bütün müşterileri getir
    public async Task<List<CustomerDto>> GetAllAsync()
    {
        var customers = await _repository.GetAllAsync();

        return customers.Adapt<List<CustomerDto>>();
    }

    // ID ile müşteri bul
    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer == null)
            return null;

        return customer.Adapt<CustomerDto>();
    }

    // Müşteri oluştur veya güncelle
    public async Task<Guid> SaveAsync(CustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) ||
            string.IsNullOrWhiteSpace(dto.LastName) ||
            string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            throw new ArgumentException(
                "Ad, soyad ve telefon numarası zorunludur.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) &&
            !MailAddress.TryCreate(dto.Email, out _))
        {
            throw new ArgumentException(
                "Geçerli bir e-posta adresi giriniz.");
        }

        Customer customer;

        if (dto.Id == Guid.Empty)
        {
            // Yeni müşteri
            customer = new Customer
            {
                CreatedDate = DateTime.UtcNow
            };

            _repository.Add(customer);
        }
        else
        {
            // Mevcut müşteriyi bul
            customer = await _repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException(
                    "Müşteri bulunamadı.");

            customer.UpdatedDate = DateTime.UtcNow;
        }

        customer.FirstName = dto.FirstName.Trim();
        customer.LastName = dto.LastName.Trim();
        customer.PhoneNumber = dto.PhoneNumber.Trim();

        customer.Email = string.IsNullOrWhiteSpace(dto.Email)
            ? null
            : dto.Email.Trim();

        await _repository.SaveChangesAsync();

        return customer.Id;
    }

    // Müşteriyi soft delete ile sil
    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer == null)
            return false;

        customer.UpdatedDate = DateTime.UtcNow;

        _repository.Delete(customer);

        await _repository.SaveChangesAsync();

        return true;
    }
}

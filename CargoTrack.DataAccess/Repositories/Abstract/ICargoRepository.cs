using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Abstract;

public interface ICargoRepository : IRepository<Cargo>
{
    Task<List<Cargo>> GetAllWithDetailsAsync();
    Task<Cargo?> GetByIdWithDetailsAsync(Guid cargoId);
    Task<Cargo?> GetByTrackingNumberAsync(string trackingNumber);
    Task<List<Cargo>> GetByCustomerIdWithDetailsAsync(Guid customerId);
    Task<List<Cargo>> GetByBranchIdWithDetailsAsync(Guid branchId);
}

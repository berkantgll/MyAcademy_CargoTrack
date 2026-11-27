
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;

namespace CargoTrack.DataAccess.Repositories.Abstract;

public interface ICargoSearchRepository
{
    Task<(List<Cargo> Items, int TotalCount)> SearchAsync(
        string? search,
        CargoStatus? status,
        Guid? branchId,
        int page,
        int pageSize);
}

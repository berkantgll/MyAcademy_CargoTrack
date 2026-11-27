
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Abstract;

public interface IAuditLogReadRepository
{
    Task<(List<AuditLog> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize);
}

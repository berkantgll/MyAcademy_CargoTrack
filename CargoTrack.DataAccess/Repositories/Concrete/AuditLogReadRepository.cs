
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Concrete;

public class AuditLogReadRepository : IAuditLogReadRepository
{
    private readonly CargoTrackDbContext _context;

    public AuditLogReadRepository(CargoTrackDbContext context)
    {
        _context = context;
    }

    public async Task<(List<AuditLog> Items, int TotalCount)>
        GetPageAsync(int page, int pageSize)
    {
        page = Math.Clamp(page, 1, 10000);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Set<AuditLog>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.ActionDate)
            .ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}

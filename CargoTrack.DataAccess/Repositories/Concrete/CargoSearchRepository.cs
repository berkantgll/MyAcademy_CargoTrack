
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Concrete;

public class CargoSearchRepository : ICargoSearchRepository
{
    private readonly CargoTrackDbContext _context;

    public CargoSearchRepository(CargoTrackDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Cargo> Items, int TotalCount)> SearchAsync(
        string? search,
        CargoStatus? status,
        Guid? branchId,
        int page,
        int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Set<Cargo>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();

            query = query.Where(
                x => x.TrackingNumber.Contains(term));
        }

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(x =>
                x.OriginBranchId == branchId.Value ||
                x.DestinationBranchId == branchId.Value);
        }

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.CreatedDate)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.OriginBranch)
            .Include(x => x.DestinationBranch)
            .ToListAsync();

        return (items, totalCount);
    }
}

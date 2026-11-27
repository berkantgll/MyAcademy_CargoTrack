
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DataAccess.Reports;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Concrete;

public class ManagerDashboardRepository : IManagerDashboardRepository
{
    private readonly CargoTrackDbContext _context;

    public ManagerDashboardRepository(CargoTrackDbContext context)
    {
        _context = context;
    }

    public async Task<ManagerDashboardSnapshot> GetAsync(Guid branchId)
    {
        // Türkiye UTC+3: bugün için UTC başlangıç ve bitiş.
        var todayTurkey = DateTime.UtcNow.AddHours(3).Date;
        var startUtc = todayTurkey.AddHours(-3);
        var endUtc = startUtc.AddDays(1);

        var branchName = await _context.Set<Branch>()
            .AsNoTracking()
            .Where(x => x.Id == branchId && !x.IsDeleted)
            .Select(x => x.BranchName)
            .FirstOrDefaultAsync();

        // Yalnızca ilgili şubeye ait kargolar.
        var cargos = _context.Set<Cargo>()
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                (x.OriginBranchId == branchId ||
                 x.DestinationBranchId == branchId));

        // Kargoların hareketleri.
        var movements =
            from movement in _context.Set<CargoMovement>()
                .AsNoTracking()
            join cargo in cargos
                on movement.CargoId equals cargo.Id
            where !movement.IsDeleted
            select new { movement, cargo };

        var result = new ManagerDashboardSnapshot
        {
            BranchName = branchName ?? "Şubem"
        };

        result.TotalCargos = await cargos.CountAsync();

        // Bugün varış şubesine ulaşan kargolar.
        result.TodayIncoming = await movements
            .Where(x =>
                x.cargo.DestinationBranchId == branchId &&
                x.movement.NewStatus == CargoStatus.AtDestinationBranch &&
                x.movement.MovementDate >= startUtc &&
                x.movement.MovementDate < endUtc)
            .Select(x => x.movement.CargoId)
            .Distinct()
            .CountAsync();

        // Bugün şubeden transfer merkezine veya dağıtıma çıkanlar.
        result.TodayOutgoing = await movements
            .Where(x =>
                x.movement.MovementDate >= startUtc &&
                x.movement.MovementDate < endUtc &&
                (
                    (x.cargo.OriginBranchId == branchId &&
                     x.movement.NewStatus == CargoStatus.AtTransferCenter)
                    ||
                    (x.cargo.DestinationBranchId == branchId &&
                     x.movement.NewStatus == CargoStatus.OutForDelivery)
                ))
            .Select(x => x.movement.CargoId)
            .Distinct()
            .CountAsync();

        result.OutForDelivery = await cargos.CountAsync(x =>
            x.DestinationBranchId == branchId &&
            x.Status == CargoStatus.OutForDelivery);

        result.DeliveredToday = await cargos.CountAsync(x =>
            x.DestinationBranchId == branchId &&
            x.Status == CargoStatus.Delivered &&
            x.DeliveredAt >= startUtc &&
            x.DeliveredAt < endUtc);

        result.DelayedCargos = await cargos.CountAsync(x =>
            x.EstimatedDeliveryDate < todayTurkey &&
            x.Status != CargoStatus.Delivered &&
            x.Status != CargoStatus.ReturnedToSender);

        result.ProblematicCargos = await cargos.CountAsync(x =>
            x.Status == CargoStatus.DeliveryFailed ||
            x.Status == CargoStatus.ReturnInProgress);

        result.RecentMovements = await movements
            .OrderByDescending(x => x.movement.MovementDate)
            .ThenByDescending(x => x.movement.Id)
            .Take(10)
            .Select(x => new ManagerMovementSnapshot
            {
                TrackingNumber = x.cargo.TrackingNumber,
                Status = x.movement.NewStatus,
                MovementDate = x.movement.MovementDate,
                Description = x.movement.Description ?? string.Empty
            })
            .ToListAsync();

        return result;
    }
}

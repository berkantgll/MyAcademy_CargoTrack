
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DataAccess.Reports;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Concrete;

public class UserDashboardRepository : IUserDashboardRepository
{
    private readonly CargoTrackDbContext _context;

    public UserDashboardRepository(CargoTrackDbContext context)
    {
        _context = context;
    }

    public async Task<UserDashboardSnapshot> GetAsync(
        Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Geçersiz müşteri.");

        var cargos = _context.Set<Cargo>()
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                (x.SenderId == customerId ||
                 x.ReceiverId == customerId));

        var result = new UserDashboardSnapshot();

        result.TotalCargos = await cargos.CountAsync();

        result.IncomingCargos = await cargos.CountAsync(
            x => x.ReceiverId == customerId);

        result.OutgoingCargos = await cargos.CountAsync(
            x => x.SenderId == customerId);

        result.ActiveCargos = await cargos.CountAsync(x =>
            x.Status != CargoStatus.Delivered &&
            x.Status != CargoStatus.ReturnedToSender);

        result.DeliveredCargos = await cargos.CountAsync(
            x => x.Status == CargoStatus.Delivered);

        result.ReturnedCargos = await cargos.CountAsync(
            x => x.Status == CargoStatus.ReturnedToSender);

        // Kullanıcının en yeni 5 kargosu.
        result.RecentCargos = await cargos
            .OrderByDescending(x => x.CreatedDate)
            .ThenByDescending(x => x.Id)
            .Take(5)
            .Select(x => new UserDashboardCargoSnapshot
            {
                Id = x.Id,
                TrackingNumber = x.TrackingNumber,
                Status = x.Status,
                Direction = x.SenderId == customerId
                    ? "Gönderilen"
                    : "Gelen",
                CreatedDate = x.CreatedDate,
                EstimatedDeliveryDate = x.EstimatedDeliveryDate
            })
            .ToListAsync();

        // Yalnızca bu müşteriye ait kargoların hareketleri.
        result.RecentMovements = await (
            from movement in _context.Set<CargoMovement>()
                .AsNoTracking()
            join cargo in cargos
                on movement.CargoId equals cargo.Id
            where !movement.IsDeleted
            orderby movement.MovementDate descending,
                    movement.Id descending
            select new UserDashboardMovementSnapshot
            {
                CargoId = cargo.Id,
                TrackingNumber = cargo.TrackingNumber,
                Status = movement.NewStatus,
                MovementDate = movement.MovementDate,
                Description = movement.Description ?? string.Empty
            })
            .Take(8)
            .ToListAsync();

        return result;
    }
}

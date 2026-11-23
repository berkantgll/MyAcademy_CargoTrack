using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Concrete;

public class CargoRepository : GenericRepository<Cargo>, ICargoRepository
{
    private readonly CargoTrackDbContext _context;

    public CargoRepository(CargoTrackDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<Cargo>> GetAllWithDetailsAsync()
    {
        return await _context.Set<Cargo>()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Sender)
            .Include(x => x.Receiver)
            .Include(x => x.DestinationBranch)
            .Include(x => x.OriginBranch)
            .ToListAsync();
    }

    public async Task<Cargo?> GetByIdWithDetailsAsync(Guid cargoId)
    {
        return await _context.Set<Cargo>()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Sender)
            .Include(x => x.Receiver)
            .Include(x => x.OriginBranch)
            .Include(x => x.DestinationBranch)
            .Include(x => x.DeliveryAddress)
            .Include(x => x.CargoMovements)
            .Include(x => x.Delivery)
            .FirstOrDefaultAsync(x => x.Id == cargoId);
    }

    public async Task<Cargo?> GetByTrackingNumberAsync(string trackingNumber)
    {
        return await _context.Set<Cargo>()
            .AsNoTracking()
            .Include(x => x.OriginBranch)
            .Include(x => x.DestinationBranch)
            .Include(x => x.CargoMovements)
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.TrackingNumber == trackingNumber);
    }


}

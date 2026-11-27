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
            .Include(x => x.DeliveryExceptions)
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

    public async Task<List<Cargo>> GetByCustomerIdWithDetailsAsync(
        Guid customerId)
    {
        return await _context.Set<Cargo>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (x.SenderId == customerId ||
                 x.ReceiverId == customerId))
            .Include(x => x.Sender)
            .Include(x => x.Receiver)
            .Include(x => x.OriginBranch)
            .Include(x => x.DestinationBranch)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Cargo>> GetByBranchIdWithDetailsAsync(
        Guid branchId)
    {
        return await _context.Set<Cargo>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (x.OriginBranchId == branchId ||
                 x.DestinationBranchId == branchId))
            .Include(x => x.Sender)
            .Include(x => x.Receiver)
            .Include(x => x.OriginBranch)
            .Include(x => x.DestinationBranch)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }



}

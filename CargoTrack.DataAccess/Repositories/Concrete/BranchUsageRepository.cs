
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Concrete;

public class BranchUsageRepository : IBranchUsageRepository
{
    private readonly CargoTrackDbContext _context;

    public BranchUsageRepository(CargoTrackDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasOpenCargosAsync(Guid branchId)
    {
        return await _context.Cargos
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                (x.OriginBranchId == branchId ||
                 x.DestinationBranchId == branchId) &&
                x.Status != CargoStatus.Delivered &&
                x.Status != CargoStatus.ReturnedToSender);
    }

    public async Task<bool> HasActiveAssignmentsAsync(Guid branchId)
    {
        bool hasEmployees = await _context.Employees
            .AsNoTracking()
            .AnyAsync(x =>
                x.BranchId == branchId &&
                x.IsActive &&
                !x.IsDeleted);

        bool hasUsers = await _context.Users
            .AsNoTracking()
            .AnyAsync(x => x.BranchId == branchId);

        return hasEmployees || hasUsers;
    }

    public async Task<bool> HasAnyReferencesAsync(Guid branchId)
    {
        bool hasCargos = await _context.Cargos
            .AsNoTracking()
            .AnyAsync(x =>
                x.OriginBranchId == branchId ||
                x.DestinationBranchId == branchId);

        bool hasMovements = await _context.CargoMovements
            .AsNoTracking()
            .AnyAsync(x => x.BranchId == branchId);

        bool hasEmployees = await _context.Employees
            .AsNoTracking()
            .AnyAsync(x => x.BranchId == branchId);

        bool hasUsers = await _context.Users
            .AsNoTracking()
            .AnyAsync(x => x.BranchId == branchId);

        return hasCargos ||
               hasMovements ||
               hasEmployees ||
               hasUsers;
    }
}

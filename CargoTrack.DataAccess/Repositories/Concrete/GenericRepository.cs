using CargoTrack.DataAccess.Context;
using CargoTrack.Entity.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Concrete;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly CargoTrackDbContext _context;

    public GenericRepository(CargoTrackDbContext context)
    {
        _context = context;
            
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Delete(T entity)
    {
        entity.IsDeleted = true;
        _context.Set<T>().Update(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Set<T>().AnyAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().Where(x => x.IsDeleted == false).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        // !x.IsDeleted == x.IsDeleted == false  AYNI ŞEYLER.
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
}

using System.Linq.Expressions;
using ContentHider.Core.Daos;
using ContentHider.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ContentHider.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly HiderDbContext _context;

    public UnitOfWork(HiderDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync<T>(T obj, CancellationToken token) where T : Dao
    {
        await _context.AddAsync(obj, token);
        await _context.SaveChangesAsync(token);
    }

    public async Task UpdateAsync<T>(T obj, CancellationToken token) where T : Dao
    {
        _context.Update(obj);
        await _context.SaveChangesAsync(token);
    }

    public async Task DeleteAsync<T>(T obj, CancellationToken token) where T : Dao
    {
        _context.Remove(obj);
        await _context.SaveChangesAsync(token);
    }

    public Task<List<T>> GetAsync<T>(Expression<Func<T, bool>>? selector = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken token = default) where T : Dao
    {
        if (include != null)
        {
            return include(_context.Set<T>())
                .Where(selector ?? (_ => true))
                .ToListAsync(token);
        }

        return _context.Set<T>()
            .Where(selector ?? (_ => true))
            .ToListAsync(token);
    }
}
using System.Linq.Expressions;
using ContentHider.Core.Daos;
using ContentHider.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ContentHider.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly HiderDbContext _context;

    public UnitOfWork(HiderDbContext context)
    {
        _context = context;
    }
    
    public Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> selector, CancellationToken token) where T : Dao
    {
        return _context.Set<T>()
            .Where(selector)
            .ToListAsync(token);
    }

    public async Task SaveAsync<T>(T obj, CancellationToken token) where T : Dao
    {
        await _context.AddAsync(obj, token);
        await _context.SaveChangesAsync(token);
    }
}
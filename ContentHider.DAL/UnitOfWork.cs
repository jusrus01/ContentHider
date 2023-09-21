using System.Linq.Expressions;
using ContentHider.Core.Entities;
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
    
    public Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> selector) where T : Dao
    {
        return _context.Set<T>()
            .Where(selector)
            .ToListAsync();
    }

    public async Task SaveAsync<T>(T obj) where T : Dao
    {
        await _context.AddAsync(obj);
        await _context.SaveChangesAsync();
    }
}
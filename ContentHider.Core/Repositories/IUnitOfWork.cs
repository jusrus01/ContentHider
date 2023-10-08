using System.Linq.Expressions;
using ContentHider.Core.Daos;
using Microsoft.EntityFrameworkCore.Query;

namespace ContentHider.Core.Repositories;

public interface IUnitOfWork
{
    Task<List<T>> GetDeprecatedAsync<T>(
        Expression<Func<T, object>>? includeExpr1 = null,
        Expression<Func<T, bool>>? selector = null,
        Expression<Func<T, object>>? includeExpr2 = null,
        CancellationToken token = default)
        where T : Dao;

    Task<List<T>> GetAsync<T>(Expression<Func<T, bool>>? selector = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken token = default) where T : Dao;

    Task SaveAsync<T>(T obj, CancellationToken token) where T : Dao;
    Task UpdateAsync<T>(T obj, CancellationToken token) where T : Dao;
    Task DeleteAsync<T>(T obj, CancellationToken token) where T : Dao;
}
using System.Linq.Expressions;
using ContentHider.Core.Daos;

namespace ContentHider.Core.Repositories;

public interface IUnitOfWork
{
    Task<List<T>> GetAsync<T>(
        Expression<Func<T, object>> includeExpr = null,
        Expression<Func<T, bool>>? selector = null,
        CancellationToken token = default)
        where T : Dao;

    Task SaveAsync<T>(T obj, CancellationToken token) where T : Dao;
    Task UpdateAsync<T>(T obj, CancellationToken token) where T : Dao;
    Task DeleteAsync<T>(T obj, CancellationToken token) where T : Dao;
}
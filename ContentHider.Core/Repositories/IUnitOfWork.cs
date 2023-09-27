using System.Linq.Expressions;
using ContentHider.Core.Daos;

namespace ContentHider.Core.Repositories;

public interface IUnitOfWork
{
    Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> selector, CancellationToken token = default) where T : Dao;
    Task SaveAsync<T>(T obj, CancellationToken token) where T : Dao;
}
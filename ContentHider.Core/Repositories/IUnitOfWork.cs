using System.Linq.Expressions;
using ContentHider.Core.Entities;

namespace ContentHider.Core.Repositories;

public interface IUnitOfWork
{
    Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> selector) where T : Dao;
    Task SaveAsync<T>(T obj) where T : Dao;
}
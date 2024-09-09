using System.Linq.Expressions;

namespace AuthenticationBase.EntityFramework.Repository;

public interface IGenericRepository<T> where T : class
{
    Task Add(T entity);
    Task Delete(T entity);
    Task Update(T entity);

    Task<T?> GetById(Guid id);
    Task<T?> GetBy(Expression<Func<T, bool>> predicate);
    IQueryable<T> GetAll();
}
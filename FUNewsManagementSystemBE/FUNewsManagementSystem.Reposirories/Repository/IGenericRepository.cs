

using System.Linq.Expressions;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(params object[] id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(params object[] id);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate);
    }
}

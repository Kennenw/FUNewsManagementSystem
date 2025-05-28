

using FUNewsManagementSystem.Reposirories.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly FunewsManagementContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(FunewsManagementContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking();
        }


        public async Task<T?> GetByIdAsync(params object[] id)
            => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(params object[] id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity is not null)
            {
                _dbSet.Remove(entity);
            }
        }
        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AsNoTracking().SingleOrDefaultAsync(predicate);
        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AsNoTracking().Where(predicate).ToListAsync();

    }
}

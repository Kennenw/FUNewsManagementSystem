using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Services
{
    public interface ICategoryService
    {
        IQueryable<Category> GetAllAsync();
        Task<Category?> GetByIdAsync(short id);
        Task AddAsync(Category entity);
        Task UpdateAsync(Category entity);
        Task DeleteAsync(short id);
        Task<bool> CheckCategoryAsync(short id);
    }
}

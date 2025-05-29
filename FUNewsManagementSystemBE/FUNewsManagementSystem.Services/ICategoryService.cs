using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Services
{
    public interface ICategoryService
    {
        IQueryable<Category> GetAllAsync();
        Task<Category?> GetByIdAsync(short id);
        Task AddAsync(CreateCategoryViewModels entity);
        Task UpdateAsync(short id, UpdateCategoryViewModels entity);
        Task DeleteAsync(short id);
        Task<bool> CheckCategoryAsync(short id);
    }
}

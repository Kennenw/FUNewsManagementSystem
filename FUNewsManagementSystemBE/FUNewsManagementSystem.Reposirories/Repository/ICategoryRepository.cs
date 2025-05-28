

using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> CheckCategoryAsync(short id);
    }
}

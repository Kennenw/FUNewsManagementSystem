
using FUNewsManagementSystem.Reposirories.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<bool> CheckCategoryAsync(short id)
        {
            return await _context.NewsArticles.AnyAsync(c => c.CategoryId == id);
        }
    }
}

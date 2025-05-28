
using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FunewsManagementContext context) : base(context)
        {
        }
    }
}

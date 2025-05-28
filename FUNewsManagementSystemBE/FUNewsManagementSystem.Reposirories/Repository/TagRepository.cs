

using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(FunewsManagementContext context) : base(context)
        {
        }
    }
}

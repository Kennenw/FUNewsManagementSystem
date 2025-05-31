
using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public interface ITagRepository : IGenericRepository<Tag>
    {
        Task<List<Tag>> SelectTags(List<int> listTags);
        Task RemoveNewsTags(string newsId);
        Task InserNewsTags(string newsId, int tagId);
    }

}

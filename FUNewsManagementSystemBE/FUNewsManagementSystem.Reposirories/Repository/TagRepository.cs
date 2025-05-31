

using FUNewsManagementSystem.Reposirories.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<List<Tag>> SelectTags(List<int> listTags)
        {
            return await _context.Tags
                                .Where(t => listTags.Contains(t.TagId))
                                .ToListAsync();
        }

        public async Task InserNewsTags(string newsId, int tagId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO NewsTag (NewsArticleId, TagId) VALUES ({0}, {1})", newsId, tagId);
        }

        public async Task RemoveNewsTags(string newsId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM NewsTag WHERE NewsArticleId = {0}", newsId);
        }
    }
}

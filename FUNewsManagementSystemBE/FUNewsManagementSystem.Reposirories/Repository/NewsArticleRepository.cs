
using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public class NewsArticleRepository : GenericRepository<NewsArticle>, INewsArticleRepository
    {
        public NewsArticleRepository(FunewsManagementContext context) : base(context)
        {
        }
    }
}

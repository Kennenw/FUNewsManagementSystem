

using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public interface INewsArticleRepository : IGenericRepository<NewsArticle>
    {
        Task<List<NewsReportItem>> GetNewsReport(DateTime startDate, DateTime endDate);
        Task<List<NewsArticleHistoryViewModels>> GetNewsModify(short accountId);
        Task<NewsArticle> GetByNewsArticleIdAsync(string id);
    }
}

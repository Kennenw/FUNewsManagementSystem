

using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Services
{
    public interface INewsArticleService
    {
        IQueryable<NewsArticle> GetAllAsync();
        Task<NewsArticle> GetByIdAsync(string id);
        Task AddAsync(NewsArticle entity);
        Task UpdateAsync(NewsArticle entity);
        Task DeleteAsync(string id);
    }
}

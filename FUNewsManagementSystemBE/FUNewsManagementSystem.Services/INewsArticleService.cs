

using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Services
{
    public interface INewsArticleService
    {
        IQueryable<NewsArticle> GetAllAsync();
        Task<NewsArticle> GetByIdAsync(string id);
        Task<NewsArticle> AddAsync(CreateNewsArticleViewModels entity);
        Task UpdateAsync(string id, UpdateNewsArticleViewModels entity);
        Task DeleteAsync(string id);
        Task<List<NewsArticleHistoryViewModels>> GetNewsModify(short accountId);
    }
}

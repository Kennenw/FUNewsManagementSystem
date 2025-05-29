

using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Services
{
    public interface INewsArticleService
    {
        IQueryable<NewsArticle> GetAllAsync();
        Task<NewsArticle> GetByIdAsync(string id);
        Task AddAsync(CreateNewsArticleViewModels entity);
        Task UpdateAsync(string id, UpdateNewsArticleViewModels entity);
        Task DeleteAsync(string id);
    }
}

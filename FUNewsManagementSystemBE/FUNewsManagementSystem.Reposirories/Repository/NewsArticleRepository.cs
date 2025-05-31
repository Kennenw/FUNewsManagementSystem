
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public class NewsArticleRepository : GenericRepository<NewsArticle>, INewsArticleRepository
    {
        public NewsArticleRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<List<NewsReportItem>> GetNewsReport (DateTime startDate, DateTime endDate)
        {
            return await _context.NewsArticles
                    .Where(a => a.CreatedDate >= startDate && a.CreatedDate <= endDate && a.NewsStatus == true) 
                    .Include(a => a.Category)
                    .Include(a => a.CreatedBy) 
                    .OrderByDescending(a => a.CreatedDate) 
                    .Select(a => new NewsReportItem
                    {
                        NewsArticleId = a.NewsArticleId,
                        Title = a.Headline,
                        CreatedDate = a.CreatedDate,
                        CategoryName = a.Category != null ? a.Category.CategoryName : "N/A",
                        AuthorName = a.CreatedBy != null ? a.CreatedBy.AccountName : "N/A"
                    })
                    .ToListAsync();
        }

        public async Task<List<NewsArticleHistoryViewModels>> GetNewsModify(short accountId)
        {
            return await _context.NewsArticles
                    .Where(a => a.CreatedById == accountId)
                    .Include(a => a.Category)
                    .Include(a => a.CreatedBy)
                    .Include(a => a.Tags)
                    .Select(a => new NewsArticleHistoryViewModels
                    {
                        NewsArticleId = a.NewsArticleId,
                        NewsTitle = a.NewsTitle,
                        Headline = a.Headline,
                        CategoryName = a.Category != null ? a.Category.CategoryName : null,
                        AuthorName = a.CreatedBy != null ? a.CreatedBy.AccountName : null,
                        CreatedDate = a.CreatedDate,
                        TagName = a.Tags.Select(t => t.TagName).ToList(),
                        NewsStatus = a.NewsStatus
                    })
                    .OrderByDescending(a => a.CreatedDate)
                    .ToListAsync();
        }

        public async Task<NewsArticle> GetByNewsArticleIdAsync(string id)
        {
            return await _context.NewsArticles
                .Include(a => a.Category)
                .Include(a => a.CreatedBy)
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(i => i.NewsArticleId.Equals(id));
        }
    }
}

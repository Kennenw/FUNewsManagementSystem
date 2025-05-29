

using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Services
{
    public class NewsArticleService : INewsArticleService
    {
        public IUnitOfWork _unitOfWork;
        public NewsArticleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddAsync(NewsArticle entity)
        {
            await _unitOfWork._newsArticleRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var itemDelete = await _unitOfWork._newsArticleRepository.GetByIdAsync(id);
            if(itemDelete != null)
            {
                itemDelete.NewsStatus = false;
                await _unitOfWork._newsArticleRepository.UpdateAsync(itemDelete);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public IQueryable<NewsArticle> GetAllAsync()
        {
            return _unitOfWork._newsArticleRepository.GetAll().Where(n => n.NewsStatus == true);
        }

        public async Task<NewsArticle?> GetByIdAsync(string id)
        {
            return await _unitOfWork._newsArticleRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(NewsArticle entity)
        {
            await _unitOfWork._newsArticleRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

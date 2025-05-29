

using AutoMapper;
using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Services
{
    public class NewsArticleService : INewsArticleService
    {
        public IUnitOfWork _unitOfWork;
        public readonly IMapper _mapper;
        public NewsArticleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task AddAsync(CreateNewsArticleViewModels entity)
        {
            NewsArticle newsArticle = _mapper.Map<NewsArticle>(entity);
            await _unitOfWork._newsArticleRepository.AddAsync(newsArticle);
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

        public async Task UpdateAsync(string id, UpdateNewsArticleViewModels entity)
        {
            var item = await _unitOfWork._newsArticleRepository.GetByIdAsync(id);
            if (item == null) return;
            _mapper.Map(entity, item);
            await _unitOfWork._newsArticleRepository.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

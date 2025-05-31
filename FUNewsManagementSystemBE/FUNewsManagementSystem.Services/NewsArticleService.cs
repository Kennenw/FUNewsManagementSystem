

using AutoMapper;
using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;
using Microsoft.EntityFrameworkCore;

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
        public async Task<NewsArticle> AddAsync(CreateNewsArticleViewModels entity)
        {

            var selectedTags = await _unitOfWork._tagRepository.SelectTags(entity.TagIds);
            NewsArticle newsArticle = _mapper.Map<NewsArticle>(entity);
            newsArticle.NewsArticleId = $"NA_{DateTime.Now:yyyyMMddHHmmss}";
            newsArticle.CreatedById = newsArticle.CreatedById;
            newsArticle.ModifiedDate = DateTime.Now;
            newsArticle.Tags = selectedTags;

            await _unitOfWork._newsArticleRepository.AddAsync(newsArticle);
            await _unitOfWork.SaveChangesAsync();
            return newsArticle;
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
            return _unitOfWork._newsArticleRepository
                .GetAll()
                .Include(a => a.Category)
                .Include(a => a.CreatedBy)
                .Include(a => a.Tags)
                .Where(n => n.NewsStatus == true);
        }

        public async Task<NewsArticle?> GetByIdAsync(string id)
        {
            return await _unitOfWork._newsArticleRepository.
                GetByNewsArticleIdAsync(id);
        }

        public async Task UpdateAsync(string id, UpdateNewsArticleViewModels entity)
        {
            try
            {

                var selectedTags = await _unitOfWork._tagRepository.SelectTags(entity.Tags);
                var item = await _unitOfWork._newsArticleRepository.GetByIdAsync(id);
                if (item == null) return;
                _mapper.Map(entity, item);
                var selectedTagIds = entity.Tags.ToHashSet();

                await _unitOfWork._tagRepository.RemoveNewsTags(id);

                foreach (var tag in selectedTagIds)
                {
                    await _unitOfWork._tagRepository.InserNewsTags(id, tag);
                }

                await _unitOfWork._newsArticleRepository.UpdateAsync(item);
                await _unitOfWork.SaveChangesAsync();
            }catch(Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật NewsArticle: " + ex.InnerException?.Message ?? ex.Message, ex);
            }
        }

        public async Task<List<NewsArticleHistoryViewModels>> GetNewsModify(short accountId)
        {
            return await _unitOfWork._newsArticleRepository.GetNewsModify(accountId);
        }
    }
}

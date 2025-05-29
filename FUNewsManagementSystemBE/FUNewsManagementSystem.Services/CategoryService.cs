
using AutoMapper;
using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Services
{
    public class CategoryService : ICategoryService
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task AddAsync(CreateCategoryViewModels entity)
        {
            Category category = _mapper.Map<Category>(entity);
            await _unitOfWork._categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CheckCategoryAsync(short id)
        {
            var category = await _unitOfWork._categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                return await _unitOfWork._categoryRepository.CheckCategoryAsync(id);
            }
            return true;
        }

        public async Task DeleteAsync(short id)
        {
            var itemDelete = await _unitOfWork._categoryRepository.GetByIdAsync(id);
            if(itemDelete != null)
            {
                itemDelete.IsActive = false;
                await _unitOfWork._categoryRepository.UpdateAsync(itemDelete);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public IQueryable<Category> GetAllAsync()
        {
            return _unitOfWork._categoryRepository.GetAll().Where(c => c.IsActive == true);
        }

        public Task<Category?> GetByIdAsync(short id)
        {
            return _unitOfWork._categoryRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(short id, UpdateCategoryViewModels entity)
        {
            var item = await _unitOfWork._categoryRepository.GetByIdAsync(id);
            if (item == null) return;
            _mapper.Map(entity, item);
            await _unitOfWork._categoryRepository.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}

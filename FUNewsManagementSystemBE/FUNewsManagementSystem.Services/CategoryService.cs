
using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Services
{
    public class CategoryService : ICategoryService
    {
        public IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddAsync(Category entity)
        {
            await _unitOfWork._categoryRepository.AddAsync(entity);
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
            await _unitOfWork._categoryRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public IQueryable<Category> GetAllAsync()
        {
            return _unitOfWork._categoryRepository.GetAll();
        }

        public Task<Category?> GetByIdAsync(short id)
        {
            return _unitOfWork._categoryRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Category entity)
        {
            await _unitOfWork._categoryRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

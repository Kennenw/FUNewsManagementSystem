
using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Services
{
    public class SystemAccountService : ISystemAccountService
    {
        public IUnitOfWork _unitOfWork;
        public SystemAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddAsync(SystemAccount entity)
        {
            await _unitOfWork._systemAccountRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            await _unitOfWork._systemAccountRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public IQueryable<SystemAccount> GetAllAsync()
        {
            return _unitOfWork._systemAccountRepository.GetAll();
        }

        public async Task<SystemAccount?> GetByIdAsync(short id)
        {
            return await _unitOfWork._systemAccountRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(SystemAccount entity)
        {
            await _unitOfWork._systemAccountRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

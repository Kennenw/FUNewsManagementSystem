

using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Services
{
    public interface ISystemAccountService
    {
        IQueryable<SystemAccount> GetAllAsync();
        Task<SystemAccount> GetByIdAsync(short id);
        Task AddAsync(CreateSystemAccountViewModel entity);
        Task UpdateAsync(short id, UpdateSystemAccountViewModel entity);
        Task DeleteAsync(short id);
        Task<SystemAccount?> LoginAsync(LoginRequestViewModel model);
        Task<bool> CheckUserAsync(short id);
    }
}

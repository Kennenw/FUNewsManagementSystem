

using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Services
{
    public interface ISystemAccountService
    {
        IQueryable<SystemAccount> GetAllAsync();
        Task<SystemAccount> GetByIdAsync(short id);
        Task AddAsync(SystemAccount entity);
        Task UpdateAsync(SystemAccount entity);
        Task DeleteAsync(short id);
        Task<SystemAccount?> LoginAsync(LoginRequestViewModel model);
    }
}

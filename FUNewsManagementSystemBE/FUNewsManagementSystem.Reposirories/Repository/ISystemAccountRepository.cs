

using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public interface ISystemAccountRepository : IGenericRepository<SystemAccount>
    {
        Task<SystemAccount?> LoginAsync(LoginRequestViewModel model);
        Task<bool> CheckUserAsync(short id);
    }
}

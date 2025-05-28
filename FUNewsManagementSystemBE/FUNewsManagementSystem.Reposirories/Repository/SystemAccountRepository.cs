

using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public class SystemAccountRepository : GenericRepository<SystemAccount>, ISystemAccountRepository
    {
        public SystemAccountRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<SystemAccount?> LoginAsync(LoginRequestViewModel model)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(sa => sa.AccountEmail.Equals(model.Email) && sa.AccountPassword.Equals(model.Password));
        }
    }
}

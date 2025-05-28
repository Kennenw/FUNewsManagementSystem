

using FUNewsManagementSystem.Reposirories.Models;

namespace FUNewsManagementSystem.Reposirories.Repository
{
    public class SystemAccountRepository : GenericRepository<SystemAccount>, ISystemAccountRepository
    {
        public SystemAccountRepository(FunewsManagementContext context) : base(context)
        {
        }
    }
}

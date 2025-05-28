

using FUNewsManagementSystem.Reposirories.Repository;

namespace FUNewsManagementSystem.Reposirories
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        ITagRepository _tagRepository { get; }
        INewsArticleRepository _newsArticleRepository { get; } 
        ISystemAccountRepository _systemAccountRepository { get; }
        ICategoryRepository _categoryRepository { get; }
        Task<int> SaveChangesAsync();
    }
}

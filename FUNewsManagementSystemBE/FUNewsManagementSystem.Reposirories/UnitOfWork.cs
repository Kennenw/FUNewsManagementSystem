
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.Repository;

namespace FUNewsManagementSystem.Reposirories
{
    public class UnitOfWork : IUnitOfWork
    {
        public ITagRepository _tagRepository { get; }
        public INewsArticleRepository _newsArticleRepository { get; }
        public ISystemAccountRepository _systemAccountRepository { get; }
        public ICategoryRepository _categoryRepository { get; }
        private readonly FunewsManagementContext _context;
        public UnitOfWork(FunewsManagementContext context, 
                            ITagRepository tagRepository, 
                            INewsArticleRepository newsArticleRepository,
                            ISystemAccountRepository systemAccountRepository,
                            ICategoryRepository categoryRepository)
        {
            _context = context;
            _tagRepository = tagRepository;
            _systemAccountRepository = systemAccountRepository;
            _categoryRepository = categoryRepository;
            _newsArticleRepository = newsArticleRepository;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}

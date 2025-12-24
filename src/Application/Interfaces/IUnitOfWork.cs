using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Resolve a repository abstraction for a given entity type.
        /// </summary>
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        IUserRepository UserRepository();
    }
}

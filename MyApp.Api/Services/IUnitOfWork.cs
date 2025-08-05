using BidFlow.Entities;
using BidFlow.Repositories;

namespace BidFlow.Services
{
    public interface IUnitOfWork : IDisposable
    {
        // Repository properties
        IRepository<User> Users { get; }
        IRepository<UserActivityLog> UserActivityLogs { get; }


        IRepository<T> Repository<T>() where T : class;


        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);


        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

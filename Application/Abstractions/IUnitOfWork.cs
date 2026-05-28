using Domain.Common;

namespace Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : BaseEntity;
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
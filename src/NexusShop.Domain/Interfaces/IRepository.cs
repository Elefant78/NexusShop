using NexusShop.Domain.Common;

namespace NexusShop.Domain.Interfaces;

/// <summary>
/// Generic repository abstraction. Lives in the Domain layer so that the
/// Application layer can depend on it without knowing about EF Core.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}

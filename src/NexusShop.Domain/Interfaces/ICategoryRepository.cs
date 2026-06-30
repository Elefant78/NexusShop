using NexusShop.Domain.Entities;

namespace NexusShop.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

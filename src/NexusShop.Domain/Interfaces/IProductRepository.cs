using NexusShop.Domain.Entities;

namespace NexusShop.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> ListByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<Product?> GetWithCategoryAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

using Microsoft.EntityFrameworkCore;
using NexusShop.Domain.Entities;
using NexusShop.Domain.Interfaces;

namespace NexusShop.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Product>> ListByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);

    public async Task<Product?> GetWithCategoryAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(p => p.Id == id, cancellationToken);
}

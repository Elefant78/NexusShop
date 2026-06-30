using Microsoft.EntityFrameworkCore;
using NexusShop.Domain.Entities;
using NexusShop.Domain.Interfaces;

namespace NexusShop.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(c => c.Id == id, cancellationToken);
}

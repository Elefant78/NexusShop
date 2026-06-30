using NexusShop.Domain.Interfaces;

namespace NexusShop.Infrastructure.Persistence.Repositories;

/// <summary>
/// Wraps the DbContext and exposes the repositories. SaveChangesAsync commits
/// all tracked changes in a single transaction.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Products = new ProductRepository(context);
        Categories = new CategoryRepository(context);
    }

    public IProductRepository Products { get; }

    public ICategoryRepository Categories { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}

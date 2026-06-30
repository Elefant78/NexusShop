namespace NexusShop.Domain.Interfaces;

/// <summary>
/// Coordinates writing changes from one or more repositories as a single
/// transaction. The Application layer calls SaveChangesAsync once per use-case.
/// </summary>
public interface IUnitOfWork
{
    IProductRepository Products { get; }

    ICategoryRepository Categories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

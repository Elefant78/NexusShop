using NexusShop.Domain.Common;
using NexusShop.Domain.Exceptions;

namespace NexusShop.Domain.Entities;

/// <summary>
/// A grouping of products (e.g. "Electronics", "Books").
/// </summary>
public class Category : BaseEntity
{
    private readonly List<Product> _products = new();

    // Required by EF Core's materialization.
    private Category() { }

    public Category(string name, string? description = null)
    {
        Rename(name);
        Description = description?.Trim();
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name must not be empty.");

        Name = name.Trim();
        Touch();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        Touch();
    }
}

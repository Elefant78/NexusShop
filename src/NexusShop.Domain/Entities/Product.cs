using NexusShop.Domain.Common;
using NexusShop.Domain.Exceptions;
using NexusShop.Domain.ValueObjects;

namespace NexusShop.Domain.Entities;

/// <summary>
/// Aggregate root representing a sellable product.
/// All state changes go through methods that enforce the business invariants,
/// so a Product can never be in an invalid state.
/// </summary>
public class Product : BaseEntity
{
    // Required by EF Core's materialization.
    private Product() { }

    public Product(string name, string? description, Money price, int stockQuantity, int categoryId)
    {
        Rename(name);
        Description = description?.Trim();
        ChangePrice(price);
        SetStock(stockQuantity);
        AssignCategory(categoryId);
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Money Price { get; private set; } = Money.Zero();

    public int StockQuantity { get; private set; }

    public bool IsActive { get; private set; } = true;

    public int CategoryId { get; private set; }

    public Category? Category { get; private set; }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name must not be empty.");
        if (name.Trim().Length > 200)
            throw new DomainException("Product name must not exceed 200 characters.");

        Name = name.Trim();
        Touch();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        Touch();
    }

    public void ChangePrice(Money price)
    {
        ArgumentNullException.ThrowIfNull(price);

        if (price.Amount <= 0)
            throw new DomainException("Product price must be greater than zero.");

        Price = price;
        Touch();
    }

    public void SetStock(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Stock quantity cannot be negative.");

        StockQuantity = quantity;
        Touch();
    }

    public void AssignCategory(int categoryId)
    {
        if (categoryId <= 0)
            throw new DomainException("A product must belong to a valid category.");

        CategoryId = categoryId;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}

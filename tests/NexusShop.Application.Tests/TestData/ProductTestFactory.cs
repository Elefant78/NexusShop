using NexusShop.Domain.Entities;
using NexusShop.Domain.ValueObjects;

namespace NexusShop.Application.Tests.TestData;

/// <summary>Convenience factory for building valid domain objects in tests.</summary>
public static class ProductTestFactory
{
    public static Product CreateProduct(
        string name = "Sample Product",
        decimal price = 19.99m,
        string currency = "CHF",
        int stock = 10,
        int categoryId = 1)
        => new(name, "A sample product.", new Money(price, currency), stock, categoryId);

    public static Category CreateCategory(string name = "Electronics")
        => new(name, "Sample category.");
}

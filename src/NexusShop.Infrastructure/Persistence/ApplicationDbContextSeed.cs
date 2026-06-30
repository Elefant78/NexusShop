using Microsoft.EntityFrameworkCore;
using NexusShop.Domain.Entities;
using NexusShop.Domain.ValueObjects;

namespace NexusShop.Infrastructure.Persistence;

/// <summary>
/// Seeds the database with demo data so the API returns meaningful results
/// the first time it is run.
/// </summary>
public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Categories.AnyAsync(cancellationToken))
            return;

        var electronics = new Category("Electronics", "Phones, laptops and gadgets.");
        var books = new Category("Books", "Printed and digital books.");

        await context.Categories.AddRangeAsync(new[] { electronics, books }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var products = new[]
        {
            new Product("Wireless Headphones", "Noise-cancelling over-ear headphones.", new Money(199.90m, "CHF"), 50, electronics.Id),
            new Product("Mechanical Keyboard", "RGB mechanical keyboard with brown switches.", new Money(129.00m, "CHF"), 80, electronics.Id),
            new Product("Clean Architecture", "Robert C. Martin - a craftsman's guide.", new Money(42.50m, "CHF"), 120, books.Id)
        };

        await context.Products.AddRangeAsync(products, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

using NexusShop.Application.DTOs;
using NexusShop.Domain.Entities;

namespace NexusShop.Application.Common.Mapping;

/// <summary>
/// Explicit, allocation-friendly mapping from domain entities to DTOs.
/// Chosen over AutoMapper: the mappings are trivial, fully compile-time checked,
/// trivially debuggable, and add no third-party dependency to the build.
/// </summary>
public static class MappingExtensions
{
    public static ProductDto ToDto(this Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price.Amount,
        Currency = product.Price.Currency,
        StockQuantity = product.StockQuantity,
        IsActive = product.IsActive,
        CategoryId = product.CategoryId,
        CategoryName = product.Category?.Name,
        CreatedAtUtc = product.CreatedAtUtc,
        UpdatedAtUtc = product.UpdatedAtUtc
    };

    public static CategoryDto ToDto(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description
    };

    public static IReadOnlyList<ProductDto> ToDtoList(this IEnumerable<Product> products)
        => products.Select(p => p.ToDto()).ToList();

    public static IReadOnlyList<CategoryDto> ToDtoList(this IEnumerable<Category> categories)
        => categories.Select(c => c.ToDto()).ToList();
}

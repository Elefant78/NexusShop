namespace NexusShop.Application.DTOs;

/// <summary>Read model returned by the API for a product.</summary>
public sealed record ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "CHF";
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; }
    public int CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

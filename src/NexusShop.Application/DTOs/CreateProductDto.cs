namespace NexusShop.Application.DTOs;

/// <summary>Write model used to create a new product.</summary>
public sealed record CreateProductDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "CHF";
    public int StockQuantity { get; init; }
    public int CategoryId { get; init; }
}

namespace NexusShop.Application.DTOs;

/// <summary>Write model used to update an existing product.</summary>
public sealed record UpdateProductDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "CHF";
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; } = true;
}

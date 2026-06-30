using FluentAssertions;
using NexusShop.Application.DTOs;
using NexusShop.Application.Validators;
using Xunit;

namespace NexusShop.Application.Tests.Validators;

public sealed class CreateProductDtoValidatorTests
{
    private readonly CreateProductDtoValidator _validator = new();

    private static CreateProductDto Valid() => new()
    {
        Name = "Valid",
        Description = "ok",
        Price = 9.99m,
        Currency = "CHF",
        StockQuantity = 3,
        CategoryId = 1
    };

    [Fact]
    public void Validate_WithValidDto_ShouldBeValid()
    {
        _validator.Validate(Valid()).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_WithNonPositivePrice_ShouldFail(decimal price)
    {
        var dto = Valid() with { Price = price };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Price));
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldFail()
    {
        var dto = Valid() with { Name = "" };
        _validator.Validate(dto).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithNegativeStock_ShouldFail()
    {
        var dto = Valid() with { StockQuantity = -1 };
        _validator.Validate(dto).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("CH")]
    [InlineData("EURO")]
    [InlineData("")]
    public void Validate_WithInvalidCurrency_ShouldFail(string currency)
    {
        var dto = Valid() with { Currency = currency };
        _validator.Validate(dto).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithInvalidCategoryId_ShouldFail()
    {
        var dto = Valid() with { CategoryId = 0 };
        _validator.Validate(dto).IsValid.Should().BeFalse();
    }
}

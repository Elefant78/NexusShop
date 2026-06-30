using FluentAssertions;
using NexusShop.Application.Tests.TestData;
using NexusShop.Domain.Exceptions;
using NexusShop.Domain.ValueObjects;
using Xunit;

namespace NexusShop.Application.Tests.Domain;

public sealed class ProductTests
{
    [Fact]
    public void ChangePrice_WithZero_ShouldThrowDomainException()
    {
        var product = ProductTestFactory.CreateProduct();

        var act = () => product.ChangePrice(new Money(0m, "CHF"));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void SetStock_WithNegativeValue_ShouldThrowDomainException()
    {
        var product = ProductTestFactory.CreateProduct();

        var act = () => product.SetStock(-1);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Rename_WithEmptyName_ShouldThrowDomainException()
    {
        var product = ProductTestFactory.CreateProduct();

        var act = () => product.Rename("   ");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void DeactivateThenActivate_ShouldToggleIsActive()
    {
        var product = ProductTestFactory.CreateProduct();

        product.Deactivate();
        product.IsActive.Should().BeFalse();

        product.Activate();
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldSetProvidedValues()
    {
        var product = ProductTestFactory.CreateProduct("Mouse", 25.50m, "CHF", 12, 3);

        product.Name.Should().Be("Mouse");
        product.Price.Amount.Should().Be(25.50m);
        product.StockQuantity.Should().Be(12);
        product.CategoryId.Should().Be(3);
        product.IsActive.Should().BeTrue();
    }
}

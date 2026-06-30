using FluentAssertions;
using NexusShop.Domain.Exceptions;
using NexusShop.Domain.ValueObjects;
using Xunit;

namespace NexusShop.Application.Tests.Domain;

public sealed class MoneyTests
{
    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrow()
    {
        var act = () => new Money(-1m, "CHF");
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("CH")]
    [InlineData("EURO")]
    [InlineData("")]
    public void Constructor_WithInvalidCurrency_ShouldThrow(string currency)
    {
        var act = () => new Money(10m, currency);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_ShouldRoundToTwoDecimalsAndUppercaseCurrency()
    {
        var money = new Money(10.125m, "chf");

        money.Amount.Should().Be(10.12m); // banker's rounding
        money.Currency.Should().Be("CHF");
    }

    [Fact]
    public void Equality_ShouldBeValueBased()
    {
        new Money(5m, "CHF").Should().Be(new Money(5m, "CHF"));
    }
}

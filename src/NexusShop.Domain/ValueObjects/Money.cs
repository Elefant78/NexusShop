using NexusShop.Domain.Exceptions;

namespace NexusShop.Domain.ValueObjects;

/// <summary>
/// Immutable value object representing a monetary amount in a given currency.
/// Value objects have no identity; two Money instances are equal when their
/// amount and currency match.
/// </summary>
public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "CHF")
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new DomainException("Currency must be a 3-letter ISO code (e.g. CHF, EUR, USD).");

        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "CHF") => new(0m, currency);

    public override string ToString() => $"{Amount:0.00} {Currency}";
}

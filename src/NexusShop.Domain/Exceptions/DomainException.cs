namespace NexusShop.Domain.Exceptions;

/// <summary>
/// Thrown when a domain invariant is violated (e.g. a negative price).
/// Keeping this in the Domain layer means the business rules are
/// self-enforcing and independent of any framework.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

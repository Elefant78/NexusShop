using FluentValidation.Results;

namespace NexusShop.Application.Common.Exceptions;

/// <summary>
/// Aggregates one or more FluentValidation failures into a single exception.
/// The API middleware translates this into an HTTP 400 response with a
/// field -> error-messages dictionary.
/// </summary>
public sealed class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}

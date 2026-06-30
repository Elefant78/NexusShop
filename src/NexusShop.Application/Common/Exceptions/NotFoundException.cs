namespace NexusShop.Application.Common.Exceptions;

/// <summary>
/// Thrown by use-cases when a requested entity does not exist.
/// The API middleware translates this into an HTTP 404 response.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.")
    {
    }
}

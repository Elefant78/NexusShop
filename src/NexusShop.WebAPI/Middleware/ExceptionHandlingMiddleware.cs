using System.Net;
using System.Text.Json;
using NexusShop.Application.Common.Exceptions;
using NexusShop.Domain.Exceptions;
using ValidationException = NexusShop.Application.Common.Exceptions.ValidationException;

namespace NexusShop.WebAPI.Middleware;

/// <summary>
/// Converts domain and application exceptions into consistent HTTP responses
/// (ProblemDetails-style JSON), keeping the controllers free of try/catch.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, title, errors) = exception switch
        {
            ValidationException v => (HttpStatusCode.BadRequest, "Validation failed.",
                (IReadOnlyDictionary<string, string[]>?)v.Errors),
            NotFoundException => (HttpStatusCode.NotFound, exception.Message,
                (IReadOnlyDictionary<string, string[]>?)null),
            DomainException => (HttpStatusCode.BadRequest, exception.Message,
                (IReadOnlyDictionary<string, string[]>?)null),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.",
                (IReadOnlyDictionary<string, string[]>?)null)
        };

        if (status == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var payload = new
        {
            status = (int)status,
            title,
            errors
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

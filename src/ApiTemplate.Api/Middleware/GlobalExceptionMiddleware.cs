using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ApiTemplate.Api.Middleware;

/// <summary>Catches all unhandled exceptions and returns RFC 7807 Problem Details responses.</summary>
public partial class GlobalExceptionMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    /// <summary>Initializes a new instance with the next middleware and logger.</summary>
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invokes the middleware, catching and mapping any exceptions.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? string.Empty;
            await HandleExceptionAsync(context, ex, correlationId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
    {
        var (status, title, extensions) = MapException(exception);

        if (status >= 500)
            LogUnhandledException(_logger, exception, correlationId);
        else
            LogHandledException(_logger, exception, correlationId);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status < 500 ? exception.Message : "An unexpected error occurred.",
            Instance = context.Request.Path
        };

        if (!string.IsNullOrEmpty(correlationId))
            problem.Extensions["correlationId"] = correlationId;

        if (extensions is not null)
            foreach (var (key, value) in extensions)
                problem.Extensions[key] = value;

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problem, SerializerOptions);

        await context.Response.WriteAsync(json);
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Unhandled exception. CorrelationId: {CorrelationId}")]
    private static partial void LogUnhandledException(ILogger logger, Exception exception, string correlationId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Handled exception. CorrelationId: {CorrelationId}")]
    private static partial void LogHandledException(ILogger logger, Exception exception, string correlationId);

    private static (int status, string title, IDictionary<string, object>? extensions) MapException(Exception exception)
        => exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found", null),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict", null),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden", null),
            DomainValidationException ve => (StatusCodes.Status422UnprocessableEntity, "Validation Error",
                new Dictionary<string, object> { ["errors"] = ve.Errors }),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", null),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", null)
        };
}

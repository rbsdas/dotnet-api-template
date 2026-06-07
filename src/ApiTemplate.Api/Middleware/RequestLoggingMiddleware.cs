namespace ApiTemplate.Api.Middleware;

/// <summary>Logs basic request and response information for every HTTP request.</summary>
public partial class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>Initializes a new instance with the next middleware and logger.</summary>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Logs the incoming request method and path, then the outgoing status code.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        LogRequestStarted(_logger, context.Request.Method, context.Request.Path);

        await _next(context);

        LogRequestCompleted(_logger, context.Request.Method, context.Request.Path, context.Response.StatusCode);
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "HTTP {Method} {Path} started")]
    private static partial void LogRequestStarted(ILogger logger, string method, PathString path);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "HTTP {Method} {Path} responded {StatusCode}")]
    private static partial void LogRequestCompleted(ILogger logger, string method, PathString path, int statusCode);
}

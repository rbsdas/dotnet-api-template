using ApiTemplate.Api.Middleware;

namespace ApiTemplate.Api.Extensions;

/// <summary>Extension methods for configuring the middleware pipeline on <see cref="WebApplication"/>.</summary>
public static class WebApplicationExtensions
{
    /// <summary>Adds the <see cref="CorrelationIdMiddleware"/> to the pipeline.</summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        => app.UseMiddleware<CorrelationIdMiddleware>();

    /// <summary>Adds the <see cref="GlobalExceptionMiddleware"/> to the pipeline.</summary>
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();
}

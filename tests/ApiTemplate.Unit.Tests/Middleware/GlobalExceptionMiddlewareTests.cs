using System.Text.Json;
using ApiTemplate.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiTemplate.Unit.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private static GlobalExceptionMiddleware CreateMiddleware(RequestDelegate next)
    {
        var logger = new Mock<ILogger<GlobalExceptionMiddleware>>();
        return new GlobalExceptionMiddleware(next, logger.Object);
    }

    private static DefaultHttpContext CreateContext()
    {
        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new System.IO.MemoryStream();
        return ctx;
    }

    public static TheoryData<Exception, int> ExceptionStatusCodes => new()
    {
        { new NotFoundException("item", 1), 404 },
        { new ConflictException("conflict"), 409 },
        { new ForbiddenException(), 403 },
        { new UnauthorizedAccessException(), 401 },
        { new InvalidOperationException("unexpected"), 500 }
    };

    [Theory]
    [MemberData(nameof(ExceptionStatusCodes))]
    public async Task Exception_MapsToCorrectStatusCode(Exception exception, int expectedStatus)
    {
        var middleware = CreateMiddleware(_ => throw exception);
        var ctx = CreateContext();

        await middleware.InvokeAsync(ctx);

        ctx.Response.StatusCode.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task UnhandledException_Returns500_WithoutStackTrace()
    {
        var middleware = CreateMiddleware(_ => throw new InvalidOperationException("boom"));
        var ctx = CreateContext();

        await middleware.InvokeAsync(ctx);

        ctx.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
        var body = await new System.IO.StreamReader(ctx.Response.Body).ReadToEndAsync();
        body.Should().NotContain("InvalidOperationException");
        body.Should().NotContain("StackTrace");
    }

    [Fact]
    public async Task Response_ContentType_IsApplicationProblemJson()
    {
        var middleware = CreateMiddleware(_ => throw new NotFoundException("x", 1));
        var ctx = CreateContext();

        await middleware.InvokeAsync(ctx);

        ctx.Response.ContentType.Should().Be("application/problem+json");
    }
}

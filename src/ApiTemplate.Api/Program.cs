using ApiTemplate.Application.Extensions;
using ApiTemplate.Infrastructure.Extensions;
using ApiTemplate.Infrastructure.Persistence;
using ApiTemplate.Api.Extensions;
using Serilog;
using Microsoft.EntityFrameworkCore;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: System.Globalization.CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting ApiTemplate API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext());

    builder.Services
        .AddApplicationServices()
        .AddInfrastructureServices(builder.Configuration)
        .AddApiServices(builder.Configuration);

    var app = builder.Build();

    app.UseCorrelationId();
    app.UseSerilogRequestLogging();
    app.UseExceptionHandler("/error");
    app.UseGlobalExceptionMiddleware();
    app.UseHttpsRedirection();
    app.UseRateLimiter();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health");

    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Staging"))
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiTemplate v1");
            options.RoutePrefix = "swagger";
        });

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pending = await db.Database.GetPendingMigrationsAsync();
        var pendingList = pending.ToList();
        if (pendingList.Count > 0)
        {
            Log.Information("Applying {Count} pending migration(s)", pendingList.Count);
            await db.Database.MigrateAsync();
        }
    }

    app.Lifetime.ApplicationStopping.Register(() => Log.Information("ApiTemplate API is shutting down"));

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ApiTemplate API terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

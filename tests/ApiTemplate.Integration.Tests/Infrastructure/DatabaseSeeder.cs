using ApiTemplate.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Integration.Tests.Infrastructure;

/// <summary>Seeds the integration test database with well-known test data.</summary>
public static class DatabaseSeeder
{
    /// <summary>Resets and re-seeds all test data in the given fixture's database.</summary>
    public static async Task SeedAsync(ApiTestFixture fixture)
    {
        using var scope = fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Examples.RemoveRange(db.Examples);
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync();
    }
}

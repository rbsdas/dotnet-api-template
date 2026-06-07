using ApiTemplate.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ApiTemplate.Infrastructure.Persistence;

/// <summary>Entity Framework Core database context for the application.</summary>
public class AppDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditInterceptor;

    /// <summary>Initializes a new instance with the given options and audit interceptor.</summary>
    public AppDbContext(DbContextOptions<AppDbContext> options, AuditableEntityInterceptor auditInterceptor)
        : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    /// <summary>Gets or sets the users table.</summary>
    public DbSet<AppUser> Users => Set<AppUser>();

    /// <summary>Gets or sets the examples table.</summary>
    public DbSet<Example> Examples => Set<Example>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }

    /// <inheritdoc/>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeUtcDateTimes();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override int SaveChanges()
    {
        NormalizeUtcDateTimes();
        return base.SaveChanges();
    }

    private void NormalizeUtcDateTimes()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue is DateTime dt && dt.Kind == DateTimeKind.Unspecified)
                    property.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }
        }
    }
}

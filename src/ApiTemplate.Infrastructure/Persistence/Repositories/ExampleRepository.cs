namespace ApiTemplate.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IExampleRepository"/>.</summary>
public class ExampleRepository : IExampleRepository
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance with the application database context.</summary>
    public ExampleRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Example?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Examples
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<PagedResult<Example>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Examples.AsNoTracking().OrderByDescending(e => e.CreatedAt);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<Example>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <inheritdoc/>
    public async Task<Example> CreateAsync(Example example, CancellationToken cancellationToken = default)
    {
        _context.Examples.Add(example);
        await _context.SaveChangesAsync(cancellationToken);
        return example;
    }

    /// <inheritdoc/>
    public async Task<Example> UpdateAsync(Example example, CancellationToken cancellationToken = default)
    {
        _context.Examples.Update(example);
        await _context.SaveChangesAsync(cancellationToken);
        return example;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var example = await _context.Examples.FindAsync([id], cancellationToken);
        if (example is not null)
        {
            _context.Examples.Remove(example);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByTitleAsync(string title, Guid? excludeId = null, CancellationToken cancellationToken = default)
        => await _context.Examples
            .AsNoTracking()
            .AnyAsync(e => e.Title == title && (excludeId == null || e.Id != excludeId), cancellationToken);
}

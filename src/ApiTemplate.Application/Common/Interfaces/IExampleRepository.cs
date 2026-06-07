namespace ApiTemplate.Application.Common.Interfaces;

/// <summary>Data access contract for <see cref="Example"/> entities.</summary>
public interface IExampleRepository
{
    /// <summary>Returns a single example by ID, or null if not found.</summary>
    Task<Example?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns a paged list of examples.</summary>
    Task<PagedResult<Example>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>Persists a new example and returns the saved entity.</summary>
    Task<Example> CreateAsync(Example example, CancellationToken cancellationToken = default);

    /// <summary>Persists changes to an existing example and returns the updated entity.</summary>
    Task<Example> UpdateAsync(Example example, CancellationToken cancellationToken = default);

    /// <summary>Deletes the example with the given ID.</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns true if an example with the given title already exists.</summary>
    Task<bool> ExistsByTitleAsync(string title, Guid? excludeId = null, CancellationToken cancellationToken = default);
}

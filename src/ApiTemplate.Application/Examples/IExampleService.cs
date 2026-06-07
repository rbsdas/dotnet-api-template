using ApiTemplate.Application.Examples.Dtos;

namespace ApiTemplate.Application.Examples;

/// <summary>Application service for managing example resources.</summary>
public interface IExampleService
{
    /// <summary>Returns a paged list of examples.</summary>
    Task<PagedResult<ExampleResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

    /// <summary>Returns a single example by ID.</summary>
    /// <exception cref="NotFoundException">Thrown when no example with <paramref name="id"/> exists.</exception>
    Task<ExampleResponse> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Creates a new example and returns the created resource.</summary>
    /// <exception cref="ConflictException">Thrown when an example with the same title already exists.</exception>
    Task<ExampleResponse> CreateAsync(CreateExampleRequest request, Guid userId, CancellationToken ct = default);

    /// <summary>Updates an existing example.</summary>
    /// <exception cref="NotFoundException">Thrown when no example with <paramref name="id"/> exists.</exception>
    /// <exception cref="ConflictException">Thrown when the new title conflicts with an existing example.</exception>
    Task<ExampleResponse> UpdateAsync(Guid id, UpdateExampleRequest request, CancellationToken ct = default);

    /// <summary>Deletes an example by ID.</summary>
    /// <exception cref="NotFoundException">Thrown when no example with <paramref name="id"/> exists.</exception>
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

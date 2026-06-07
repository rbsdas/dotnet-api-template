using ApiTemplate.Application.Examples.Dtos;
using ApiTemplate.Application.Examples.Validators;

namespace ApiTemplate.Application.Examples;

/// <summary>Implements CRUD operations for example resources.</summary>
public class ExampleService : IExampleService
{
    private readonly IExampleRepository _repository;
    private readonly IValidator<CreateExampleRequest> _createValidator;
    private readonly IValidator<UpdateExampleRequest> _updateValidator;

    /// <summary>Initializes a new instance with required dependencies.</summary>
    public ExampleService(
        IExampleRepository repository,
        IValidator<CreateExampleRequest> createValidator,
        IValidator<UpdateExampleRequest> updateValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<ExampleResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var result = await _repository.GetPagedAsync(page, pageSize, ct);

        return new PagedResult<ExampleResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    /// <inheritdoc/>
    public async Task<ExampleResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var example = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Example), id);

        return MapToResponse(example);
    }

    /// <inheritdoc/>
    public async Task<ExampleResponse> CreateAsync(CreateExampleRequest request, Guid userId, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new DomainValidationException(validation.Errors);

        if (await _repository.ExistsByTitleAsync(request.Title, null, ct))
            throw new ConflictException($"An example with the title '{request.Title}' already exists.");

        var example = new Example
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            CreatedByUserId = userId
        };

        var created = await _repository.CreateAsync(example, ct);
        return MapToResponse(created);
    }

    /// <inheritdoc/>
    public async Task<ExampleResponse> UpdateAsync(Guid id, UpdateExampleRequest request, CancellationToken ct = default)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new DomainValidationException(validation.Errors);

        var example = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Example), id);

        if (await _repository.ExistsByTitleAsync(request.Title, id, ct))
            throw new ConflictException($"An example with the title '{request.Title}' already exists.");

        example.Title = request.Title;
        example.Description = request.Description;
        example.Status = request.Status;

        var updated = await _repository.UpdateAsync(example, ct);
        return MapToResponse(updated);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var exists = await _repository.GetByIdAsync(id, ct);
        if (exists is null)
            throw new NotFoundException(nameof(Example), id);

        await _repository.DeleteAsync(id, ct);
    }

    private static ExampleResponse MapToResponse(Example example) => new(
        example.Id,
        example.Title,
        example.Description,
        example.Status,
        example.CreatedByUserId,
        example.CreatedAt,
        example.UpdatedAt
    );
}

using Asp.Versioning;

namespace ApiTemplate.Api.Controllers.V1;

/// <summary>CRUD endpoints for example resources.</summary>
[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/examples")]
[Authorize]
public class ExampleController : ControllerBase
{
    private readonly IExampleService _exampleService;
    private readonly ICurrentUserService _currentUser;

    /// <summary>Initializes a new instance with required services.</summary>
    public ExampleController(IExampleService exampleService, ICurrentUserService currentUser)
    {
        _exampleService = exampleService;
        _currentUser = currentUser;
    }

    /// <summary>Returns a paged list of examples.</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiTemplate.Application.Common.Models.PagedResult<ExampleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _exampleService.GetPagedAsync(page, pageSize, ct);
        return Ok(result);
    }

    /// <summary>Returns a single example by ID.</summary>
    /// <param name="id">The example's unique identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExampleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await _exampleService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>Creates a new example.</summary>
    /// <param name="request">The creation payload.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ExampleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateExampleRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        var result = await _exampleService.CreateAsync(request, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Updates an existing example.</summary>
    /// <param name="id">The example's unique identifier.</param>
    /// <param name="request">The update payload.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ExampleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExampleRequest request, CancellationToken ct = default)
    {
        var result = await _exampleService.UpdateAsync(id, request, ct);
        return Ok(result);
    }

    /// <summary>Deletes an example by ID.</summary>
    /// <param name="id">The example's unique identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await _exampleService.DeleteAsync(id, ct);
        return NoContent();
    }
}

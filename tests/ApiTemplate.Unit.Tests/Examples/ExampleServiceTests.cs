namespace ApiTemplate.Unit.Tests.Examples;

public class ExampleServiceTests
{
    private readonly Mock<IExampleRepository> _repoMock = new();
    private readonly IValidator<CreateExampleRequest> _createValidator = new CreateExampleRequestValidator();
    private readonly IValidator<UpdateExampleRequest> _updateValidator = new UpdateExampleRequestValidator();
    private ExampleService CreateService() => new(_repoMock.Object, _createValidator, _updateValidator);

    private static Example MakeExample(string title = "Test") => new()
    {
        Title = title,
        Description = "A description",
        Status = ExampleStatus.Draft,
        CreatedByUserId = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task GetById_WhenExists_ReturnsDto()
    {
        var example = MakeExample();
        _repoMock.Setup(r => r.GetByIdAsync(example.Id, default)).ReturnsAsync(example);

        var sut = CreateService();
        var result = await sut.GetByIdAsync(example.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(example.Id);
        result.Title.Should().Be(example.Title);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ThrowsNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Example?)null);
        var sut = CreateService();

        var act = async () => await sut.GetByIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedDto()
    {
        var request = new CreateExampleRequest("Unique Title", "A long enough description");
        var userId = Guid.NewGuid();
        _repoMock.Setup(r => r.ExistsByTitleAsync(request.Title, null, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Example>(), default))
                 .ReturnsAsync((Example e, CancellationToken _) => e);

        var sut = CreateService();
        var result = await sut.CreateAsync(request, userId);

        result.Title.Should().Be(request.Title);
        result.CreatedByUserId.Should().Be(userId);
    }

    [Fact]
    public async Task Create_WithDuplicateTitle_ThrowsConflictException()
    {
        var request = new CreateExampleRequest("Duplicate", "Description");
        _repoMock.Setup(r => r.ExistsByTitleAsync(request.Title, null, default)).ReturnsAsync(true);

        var sut = CreateService();
        var act = async () => await sut.CreateAsync(request, Guid.NewGuid());

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Update_WhenNotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        var request = new UpdateExampleRequest("New Title", "New Description", ExampleStatus.Active);
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Example?)null);

        var sut = CreateService();
        var act = async () => await sut.UpdateAsync(id, request);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Delete_WhenNotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Example?)null);

        var sut = CreateService();
        var act = async () => await sut.DeleteAsync(id);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}

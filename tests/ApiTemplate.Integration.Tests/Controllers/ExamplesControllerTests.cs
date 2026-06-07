using ApiTemplate.Application.Common.Models;

namespace ApiTemplate.Integration.Tests.Controllers;

[Collection("Integration")]
public class ExamplesControllerTests : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _fixture;

    public ExamplesControllerTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<string> GetTokenAsync()
    {
        var client = _fixture.CreateUnauthenticatedClient();
        var email = $"ex_{Guid.NewGuid()}@test.com";
        var reg = await client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterRequest(email, "Password1!", "Password1!"));
        var auth = await reg.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.AccessToken;
    }

    [Fact]
    public async Task GetAll_Returns200WithPagedList()
    {
        var token = await GetTokenAsync();
        var client = _fixture.CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/examples");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<ExampleResponse>>();
        body.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        var token = await GetTokenAsync();
        var client = _fixture.CreateAuthenticatedClient(token);

        var response = await client.GetAsync($"/api/v1/examples/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidBody_Returns201WithLocationHeader()
    {
        var token = await GetTokenAsync();
        var client = _fixture.CreateAuthenticatedClient(token);
        var request = new CreateExampleRequest($"Title {Guid.NewGuid()}", "A sufficient description");

        var response = await client.PostAsJsonAsync("/api/v1/examples", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_WithInvalidBody_Returns422()
    {
        var token = await GetTokenAsync();
        var client = _fixture.CreateAuthenticatedClient(token);
        var request = new CreateExampleRequest(string.Empty, string.Empty);

        var response = await client.PostAsJsonAsync("/api/v1/examples", request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Delete_WithoutAuth_Returns401()
    {
        var client = _fixture.CreateUnauthenticatedClient();

        var response = await client.DeleteAsync($"/api/v1/examples/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

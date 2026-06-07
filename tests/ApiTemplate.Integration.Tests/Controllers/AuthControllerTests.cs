namespace ApiTemplate.Integration.Tests.Controllers;

[Collection("Integration")]
public class AuthControllerTests : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _fixture;

    public AuthControllerTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Register_WithValidRequest_Returns201()
    {
        var client = _fixture.CreateUnauthenticatedClient();
        var request = new RegisterRequest(
            $"user_{Guid.NewGuid()}@test.com",
            "Password1!",
            "Password1!");

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        var client = _fixture.CreateUnauthenticatedClient();
        var email = $"dup_{Guid.NewGuid()}@test.com";
        var request = new RegisterRequest(email, "Password1!", "Password1!");

        await client.PostAsJsonAsync("/api/v1/auth/register", request);
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        var client = _fixture.CreateUnauthenticatedClient();
        var request = new LoginRequest("nobody@test.com", "WrongPassword1");

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

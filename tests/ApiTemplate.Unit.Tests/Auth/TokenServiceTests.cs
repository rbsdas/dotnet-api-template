using ApiTemplate.Infrastructure.Identity;
using Microsoft.Extensions.Options;

namespace ApiTemplate.Unit.Tests.Auth;

public class TokenServiceTests
{
    private readonly JwtOptions _options = new()
    {
        Secret = "test-secret-key-that-is-at-least-32-chars-long",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpiryMinutes = 60
    };

    private TokenService CreateService() =>
        new(Options.Create(_options));

    private static AppUser MakeUser() => new()
    {
        Email = "test@example.com",
        PasswordHash = "hash"
    };

    [Fact]
    public void GenerateToken_ReturnsValidJwt()
    {
        var sut = CreateService();
        var token = sut.GenerateToken(MakeUser());

        token.Should().NotBeNullOrWhiteSpace();
        token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public void GenerateToken_ContainsCorrectClaims()
    {
        var user = MakeUser();
        var sut = CreateService();
        var token = sut.GenerateToken(user);

        var userId = sut.ValidateToken(token);
        userId.Should().Be(user.Id);
    }

    [Fact]
    public void ValidateToken_WithExpiredToken_ReturnsNull()
    {
        var expiredOptions = new JwtOptions
        {
            Secret = _options.Secret,
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            ExpiryMinutes = -1
        };
        var sut = new TokenService(Options.Create(expiredOptions));
        var token = sut.GenerateToken(MakeUser());

        var result = sut.ValidateToken(token);

        result.Should().BeNull();
    }
}

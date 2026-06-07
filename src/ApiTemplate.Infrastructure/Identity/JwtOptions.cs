namespace ApiTemplate.Infrastructure.Identity;

/// <summary>Configuration options for JWT token generation and validation.</summary>
public class JwtOptions
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "Jwt";

    /// <summary>Gets or sets the HMAC-SHA256 signing secret. Must be at least 32 characters.</summary>
    public string Secret { get; init; } = string.Empty;

    /// <summary>Gets or sets the token issuer claim value.</summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>Gets or sets the token audience claim value.</summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>Gets or sets how long in minutes the access token is valid.</summary>
    public int ExpiryMinutes { get; init; } = 60;

    /// <summary>Gets or sets how long in days the refresh token is valid.</summary>
    public int RefreshExpiryDays { get; init; } = 7;
}

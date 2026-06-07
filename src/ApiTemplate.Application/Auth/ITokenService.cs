namespace ApiTemplate.Application.Auth;

/// <summary>Generates and validates JWT access tokens.</summary>
public interface ITokenService
{
    /// <summary>Generates a signed JWT access token for the given user.</summary>
    string GenerateToken(AppUser user);

    /// <summary>Returns the user ID embedded in the token, or null if the token is invalid or expired.</summary>
    Guid? ValidateToken(string token);
}

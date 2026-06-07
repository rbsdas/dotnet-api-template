namespace ApiTemplate.Application.Auth.Dtos;

/// <summary>Response payload returned after successful authentication.</summary>
public record AuthResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    Guid UserId,
    string Email
);

using ApiTemplate.Application.Auth.Dtos;

namespace ApiTemplate.Application.Auth;

/// <summary>Handles user registration and authentication flows.</summary>
public interface IAuthService
{
    /// <summary>Registers a new user account.</summary>
    /// <exception cref="ConflictException">Thrown when a user with the given email already exists.</exception>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

    /// <summary>Authenticates a user and returns an access token.</summary>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

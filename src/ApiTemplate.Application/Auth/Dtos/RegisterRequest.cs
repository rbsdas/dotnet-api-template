namespace ApiTemplate.Application.Auth.Dtos;

/// <summary>Request payload for registering a new user account.</summary>
public record RegisterRequest(string Email, string Password, string ConfirmPassword);

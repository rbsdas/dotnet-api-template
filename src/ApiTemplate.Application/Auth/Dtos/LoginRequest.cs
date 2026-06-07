namespace ApiTemplate.Application.Auth.Dtos;

/// <summary>Request payload for authenticating an existing user.</summary>
public record LoginRequest(string Email, string Password);

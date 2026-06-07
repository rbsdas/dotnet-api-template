namespace ApiTemplate.Application.Common.Interfaces;

/// <summary>Provides access to the currently authenticated user's identity.</summary>
public interface ICurrentUserService
{
    /// <summary>Gets the current user's ID, or null if unauthenticated.</summary>
    Guid? UserId { get; }

    /// <summary>Gets the current user's email, or null if unauthenticated.</summary>
    string? Email { get; }

    /// <summary>Gets whether the current request is authenticated.</summary>
    bool IsAuthenticated { get; }
}

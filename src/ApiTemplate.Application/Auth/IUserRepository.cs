namespace ApiTemplate.Application.Auth;

/// <summary>Data access contract for <see cref="AppUser"/> entities.</summary>
public interface IUserRepository
{
    /// <summary>Returns a user by their email address, or null if not found.</summary>
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Returns a user by their ID, or null if not found.</summary>
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Persists a new user and returns the saved entity.</summary>
    Task<AppUser> CreateAsync(AppUser user, CancellationToken cancellationToken = default);
}

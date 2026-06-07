using ApiTemplate.Application.Common.Interfaces;

namespace ApiTemplate.Infrastructure.Identity;

/// <summary>BCrypt implementation of <see cref="IPasswordHasher"/> with work factor 12.</summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    /// <inheritdoc/>
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    /// <inheritdoc/>
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}

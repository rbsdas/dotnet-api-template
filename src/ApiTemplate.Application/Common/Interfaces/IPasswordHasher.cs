namespace ApiTemplate.Application.Common.Interfaces;

/// <summary>Provides password hashing and verification without exposing the algorithm to Application.</summary>
public interface IPasswordHasher
{
    /// <summary>Returns a hashed representation of <paramref name="password"/>.</summary>
    string Hash(string password);

    /// <summary>Returns true if <paramref name="password"/> matches <paramref name="hash"/>.</summary>
    bool Verify(string password, string hash);
}

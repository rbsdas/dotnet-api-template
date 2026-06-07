namespace ApiTemplate.Domain.Entities;

/// <summary>Represents an application user with credentials.</summary>
public class AppUser : AuditableEntity
{
    /// <summary>Gets or sets the user's unique email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the BCrypt-hashed password.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Gets or sets the examples created by this user.</summary>
    public ICollection<Example> Examples { get; set; } = new List<Example>();
}

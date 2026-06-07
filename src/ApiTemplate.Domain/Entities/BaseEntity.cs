namespace ApiTemplate.Domain.Entities;

/// <summary>Base class for all domain entities with a generated GUID primary key.</summary>
public abstract class BaseEntity
{
    /// <summary>Gets the unique identifier for this entity.</summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();
}

namespace ApiTemplate.Domain.Entities;

/// <summary>Base class for entities that track creation and modification timestamps.</summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>Gets or sets the UTC timestamp when this entity was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the UTC timestamp when this entity was last updated.</summary>
    public DateTime UpdatedAt { get; set; }
}

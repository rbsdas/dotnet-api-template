namespace ApiTemplate.Domain.Entities;

/// <summary>Represents an example resource demonstrating the full CRUD stack.</summary>
public class Example : AuditableEntity
{
    /// <summary>Gets or sets the title of the example.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the detailed description of the example.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the current status of the example.</summary>
    public ExampleStatus Status { get; set; } = ExampleStatus.Draft;

    /// <summary>Gets or sets the ID of the user who created this example.</summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>Gets or sets the user who created this example.</summary>
    public AppUser? CreatedByUser { get; set; }
}

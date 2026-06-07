namespace ApiTemplate.Domain.Enums;

/// <summary>Represents the lifecycle state of an <see cref="Example"/> entity.</summary>
public enum ExampleStatus
{
    /// <summary>The example is in draft and not yet published.</summary>
    Draft = 0,

    /// <summary>The example has been published and is active.</summary>
    Active = 1,

    /// <summary>The example has been archived and is no longer active.</summary>
    Archived = 2
}

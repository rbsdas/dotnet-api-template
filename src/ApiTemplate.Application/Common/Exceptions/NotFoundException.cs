namespace ApiTemplate.Application.Common.Exceptions;

/// <summary>Thrown when a requested resource cannot be found.</summary>
public class NotFoundException : Exception
{
    /// <summary>Initializes a new instance with a message identifying the missing resource.</summary>
    public NotFoundException(string name, object key)
        : base($"Entity '{name}' with key '{key}' was not found.") { }

    /// <summary>Initializes a new instance with a custom message.</summary>
    public NotFoundException(string message) : base(message) { }
}

namespace ApiTemplate.Application.Common.Exceptions;

/// <summary>Thrown when an operation would create a conflicting resource state.</summary>
public class ConflictException : Exception
{
    /// <summary>Initializes a new instance with a conflict description.</summary>
    public ConflictException(string message) : base(message) { }
}

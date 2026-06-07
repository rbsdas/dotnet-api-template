namespace ApiTemplate.Application.Common.Exceptions;

/// <summary>Thrown when the current user is authenticated but lacks permission to perform an action.</summary>
public class ForbiddenException : Exception
{
    /// <summary>Initializes a new instance with a default forbidden message.</summary>
    public ForbiddenException() : base("You do not have permission to perform this action.") { }

    /// <summary>Initializes a new instance with a custom message.</summary>
    public ForbiddenException(string message) : base(message) { }
}

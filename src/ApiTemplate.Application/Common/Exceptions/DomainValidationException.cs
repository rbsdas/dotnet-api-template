namespace ApiTemplate.Application.Common.Exceptions;

/// <summary>Thrown when one or more validation rules fail.</summary>
public class DomainValidationException : Exception
{
    /// <summary>Gets the dictionary of field-level validation errors.</summary>
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>Initializes a new instance from FluentValidation failures.</summary>
    public DomainValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    /// <summary>Initializes a new instance with a pre-built errors dictionary.</summary>
    public DomainValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}

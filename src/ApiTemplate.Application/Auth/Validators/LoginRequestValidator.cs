using ApiTemplate.Application.Auth.Dtos;

namespace ApiTemplate.Application.Auth.Validators;

/// <summary>Validation rules for <see cref="LoginRequest"/>.</summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>Initializes a new instance with all validation rules.</summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

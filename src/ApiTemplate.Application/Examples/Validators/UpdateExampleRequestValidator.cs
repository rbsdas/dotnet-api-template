using ApiTemplate.Application.Examples.Dtos;

namespace ApiTemplate.Application.Examples.Validators;

/// <summary>Validation rules for <see cref="UpdateExampleRequest"/>.</summary>
public class UpdateExampleRequestValidator : AbstractValidator<UpdateExampleRequest>
{
    /// <summary>Initializes a new instance with all validation rules.</summary>
    public UpdateExampleRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid ExampleStatus value.");
    }
}

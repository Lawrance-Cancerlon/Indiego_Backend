using System;
using FluentValidation;
using Indiego_Backend.Contracts;

namespace Indiego_Backend.Validators;

public class CreatePostValidator : AbstractValidator<CreatePostContract>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(3, 100).WithMessage("Title must be between 3 and 100 characters long.");
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text is required.")
            .MaximumLength(500).WithMessage("Text must not exceed 500 characters.");
    }
}

public class UpdatePostValidator : AbstractValidator<UpdatePostContract>
{
    public UpdatePostValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Title), () => {
            RuleFor(x => x.Title)
                .Length(3, 100).WithMessage("Title must be between 3 and 100 characters long.");
        });
        When(x => !string.IsNullOrEmpty(x.Text), () => {
            RuleFor(x => x.Text)
                .MaximumLength(500).WithMessage("Text must not exceed 500 characters.");
        });
    }
}
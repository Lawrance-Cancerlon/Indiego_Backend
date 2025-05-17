using System;
using FluentValidation;
using Indiego_Backend.Contracts;

namespace Indiego_Backend.Validators;

public class CreateReviewValidator : AbstractValidator<CreateReviewContract>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text is required.")
            .MaximumLength(100).WithMessage("Text must not exceed 500 characters.");
        RuleFor(x => x.Rating)
            .NotEmpty().WithMessage("Rating is required.")
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
    }
}

public class UpdateReviewValidator : AbstractValidator<UpdateReviewContract>
{
    public UpdateReviewValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Text), () => {
            RuleFor(x => x.Text)
                .MaximumLength(100).WithMessage("Text must not exceed 500 characters.");
        });
        When(x => x.Rating != null, () => {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        });
    }
}

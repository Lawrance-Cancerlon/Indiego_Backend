using System;
using FluentValidation;
using Indiego_Backend.Contracts;

namespace Indiego_Backend.Validators;

public class CreateSubscriptionTypeValidator : AbstractValidator<CreateSubscriptionTypeContract>
{
    public CreateSubscriptionTypeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 100).WithMessage("Name must be between 3 and 100 characters long.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Length(10, 500).WithMessage("Description must be between 10 and 500 characters long.");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Duration)
            .NotEmpty().WithMessage("Duration is required.")
            .GreaterThan(0).WithMessage("Duration must be greater than 0.");
    }
}

public class UpdateSubscriptionTypeValidator : AbstractValidator<UpdateSubscriptionTypeContract>
{
    public UpdateSubscriptionTypeValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Name), () => {
            RuleFor(x => x.Name)
                .Length(3, 100).WithMessage("Name must be between 3 and 100 characters long.");
        });
        When(x => !string.IsNullOrEmpty(x.Description), () => {
            RuleFor(x => x.Description)
                .Length(10, 500).WithMessage("Description must be between 10 and 500 characters long.");
        });
        When(x => x.Price != null, () => {
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");
        });
        When(x => x.Duration != null, () => {
            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than 0.");
        });
    }
}
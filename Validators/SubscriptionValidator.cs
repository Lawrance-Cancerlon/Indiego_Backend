using System;
using FluentValidation;
using Indiego_Backend.Contracts;

namespace Indiego_Backend.Validators;

public class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionContract>
{

    public CreateSubscriptionValidator()
    {
        RuleFor(x => x.SubscriptionTypeId)
            .NotEmpty().WithMessage("SubscriptionTypeId is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("SubscriptionTypeId must be a valid ID format.");
    }
}

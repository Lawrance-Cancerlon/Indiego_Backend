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
            .Must(id => MongoDB.Bson.ObjectId.TryParse(id.ToString(), out _)).WithMessage("Subscription Type ID must be a valid MongoDB ObjectId.");
    }
}

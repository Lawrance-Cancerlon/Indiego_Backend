using System;
using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Utilities;

namespace Indiego_Backend.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerContract>
{
    public CreateCustomerValidator()
    {
        Include(new CreateUserValidator());
        RuleFor(x => DatetimeUtility.FromUnixTimestampString(x.BirthDate))
            .NotEmpty().WithMessage("BirthDate is required.")
            .LessThanOrEqualTo(DateTime.Now.AddYears(-13)).WithMessage("You must be at least 13 years old to create an account.");
    }
}

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerContract>
{
    public UpdateCustomerValidator()
    {
        Include(new UpdateUserValidator());
        When(x => x.BirthDate != null, () => {
            RuleFor(x => DatetimeUtility.FromUnixTimestampString(x.BirthDate!))
                .LessThanOrEqualTo(DateTime.Now.AddYears(-13)).WithMessage("You must be at least 13 years old to create an account.");
        });
    }
}

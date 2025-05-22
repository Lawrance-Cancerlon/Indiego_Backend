using System;
using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;
using Indiego_Backend.Utilities;

namespace Indiego_Backend.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerContract>
{
    public CreateCustomerValidator(IUserRepository<User> userRepository)
    {
        Include(new CreateUserValidator(userRepository));
        RuleFor(x => DatetimeUtility.FromUnixTimestampString(x.BirthDate))
            .NotEmpty().WithMessage("BirthDate is required.")
            .LessThanOrEqualTo(DateTime.Now.AddYears(-13)).WithMessage("You must be at least 13 years old to create an account.");
    }
}

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerContract>
{
    public UpdateCustomerValidator(IUserRepository<User> userRepository)
    {
        Include(new UpdateUserValidator(userRepository));
        When(x => x.BirthDate != null, () => {
            RuleFor(x => DatetimeUtility.FromUnixTimestampString(x.BirthDate!))
                .LessThanOrEqualTo(DateTime.Now.AddYears(-13)).WithMessage("You must be at least 13 years old to create an account.");
        });
    }
}

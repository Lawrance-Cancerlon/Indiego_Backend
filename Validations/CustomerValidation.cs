using System;
using FluentValidation;
using Indiego_Backend.Contracts.Users;

namespace Indiego_Backend.Validations;

public class CreateCustomerValidation : AbstractValidator<CreateCustomerContract>
{
    public CreateCustomerValidation()
    {
        Include(new CreateUserValidation());
        RuleFor(p => p.BirthDate).NotNull().LessThanOrEqualTo(DateTime.Now.AddYears(-13)).WithMessage("BirthDate must be greater than 13 years");
    }
}

public class UpdateCustomerValidation : AbstractValidator<UpdateCustomerContract>
{
    public UpdateCustomerValidation()
    {
        Include(new UpdateUserValidation());
        RuleFor(p => p.BirthDate).LessThanOrEqualTo(DateTime.Now.AddYears(-13)).WithMessage("BirthDate must be greater than 13 years");
    }
}
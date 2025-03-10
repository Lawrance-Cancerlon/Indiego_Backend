using System;
using FluentValidation;
using Indiego_Backend.Contracts.Users;

namespace Indiego_Backend.Validations;

public class CreateCustomerValidation : AbstractValidator<CreateCustomerContract>
{
    public CreateCustomerValidation()
    {
        Include(new CreateUserValidation());
        RuleFor(p => p.BirthDate).LessThanOrEqualTo(DateTime.Now.AddYears(-18)).WithMessage("BirthDate must be greater than 18 years");
    }
}

using System;
using FluentValidation;
using Indiego_Backend.Contracts.Users;

namespace Indiego_Backend.Validations;

public class CreateUserValidation : AbstractValidator<CreateUserContract>
{
    public CreateUserValidation()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(p => p.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required");
        RuleFor(p => p.Password).NotEmpty().MinimumLength(8).WithMessage("Password must be at least 8 characters long");
    }
}

public class UpdateUserValidation : AbstractValidator<UpdateUserContract>
{
    public UpdateUserValidation()
    {
        RuleFor(p => p.Email).EmailAddress().WithMessage("Valid email is required");
        RuleFor(p => p.Password).MinimumLength(8).WithMessage("Password must be at least 8 characters long");
    }
}

using System;
using FluentValidation;
using Indiego_Backend.Contracts.Users;

namespace Indiego_Backend.Validations;

public class CreateDeveloperValidation : AbstractValidator<CreateDeveloperContract>
{
    public CreateDeveloperValidation()
    {
        RuleFor(p => p.Bank).NotEmpty().WithMessage("Bank is required");
        RuleFor(p => p.AccountNumber).NotEmpty().WithMessage("AccountNumber is required");
        RuleFor(p => p.AccountNumber).Matches(@"^\d*$").WithMessage("AccountNumber must be numeric");
        RuleFor(p => p.AccountName).NotEmpty().WithMessage("AccountName is required");
    }
}

public class UpdateDeveloperValidation : AbstractValidator<UpdateDeveloperContract>
{
    public UpdateDeveloperValidation()
    {
        Include(new UpdateCustomerValidation());
        RuleFor(p => p.AccountNumber).Matches(@"^\d*$").WithMessage("AccountNumber must be numeric");
    }
}
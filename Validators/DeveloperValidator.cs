using System;
using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Validators;

public class CreateDeveloperValidator : AbstractValidator<CreateDeveloperContract>
{
    public CreateDeveloperValidator()
    {
        RuleFor(x => x.DevName)
            .NotEmpty().WithMessage("DevName is required.")
            .MaximumLength(50).WithMessage("DevName must not exceed 50 characters.");
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required.")
            .MaximumLength(100).WithMessage("FullName must not exceed 100 characters.");
        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("TaxId is required.")
            .MaximumLength(20).WithMessage("TaxId must not exceed 20 characters.");
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(50).WithMessage("Country must not exceed 50 characters.");
    }
}

public class UpdateDeveloperValidator : AbstractValidator<UpdateDeveloperContract>
{
    public UpdateDeveloperValidator(IUserRepository<User> userRepository)
    {
        Include(new UpdateCustomerValidator(userRepository));
        When(x => x.DevName != null, () => {
            RuleFor(x => x.DevName)
                .NotEmpty().WithMessage("DevName is required.")
                .MaximumLength(50).WithMessage("DevName must not exceed 50 characters.");
        });
        When(x => x.FullName != null, () => {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(100).WithMessage("FullName must not exceed 100 characters.");
        });
        When(x => x.TaxId != null, () => {
            RuleFor(x => x.TaxId)
                .NotEmpty().WithMessage("TaxId is required.")
                .MaximumLength(20).WithMessage("TaxId must not exceed 20 characters.");
        });
        When(x => x.Country != null, () => {
            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(50).WithMessage("Country must not exceed 50 characters.");
        });
    }
}
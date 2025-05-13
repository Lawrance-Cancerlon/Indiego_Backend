using System;
using FluentValidation;
using Indiego_Backend.Contracts;

namespace Indiego_Backend.Validators;

public class CreateAdminValidator : AbstractValidator<CreateAdminContract>
{
    public CreateAdminValidator()
    {
        Include(new CreateUserValidator());
        RuleFor(x => x.CanManageAdmins)
            .NotNull().WithMessage("CanManageAdmins is required.");
        RuleFor(x => x.CanManageUsers)
            .NotNull().WithMessage("CanManageUsers is required.");
        RuleFor(x => x.CanManageGames)
            .NotNull().WithMessage("CanManageGames is required.");
        RuleFor(x => x.CanManagePosts)
            .NotNull().WithMessage("CanManagePosts is required.");
        RuleFor(x => x.CanManageReviews)
            .NotNull().WithMessage("CanManageReviews is required.");
        RuleFor(x => x.CanManageTransactions)
            .NotNull().WithMessage("CanManageTransactions is required.");
    }
}

public class UpdateAdminValidator : AbstractValidator<UpdateAdminContract>
{
    public UpdateAdminValidator()
    {
        Include(new UpdateUserValidator());
        When(x => x.CanManageAdmins != null, () => {
            RuleFor(x => x.CanManageAdmins)
                .NotNull().WithMessage("CanManageAdmins is required.");
        });
        When(x => x.CanManageUsers != null, () => {
            RuleFor(x => x.CanManageUsers)
                .NotNull().WithMessage("CanManageUsers is required.");
        });
        When(x => x.CanManageGames != null, () => {
            RuleFor(x => x.CanManageGames)
                .NotNull().WithMessage("CanManageGames is required.");
        });
        When(x => x.CanManagePosts != null, () => {
            RuleFor(x => x.CanManagePosts)
                .NotNull().WithMessage("CanManagePosts is required.");
        });
        When(x => x.CanManageReviews != null, () => {
            RuleFor(x => x.CanManageReviews)
                .NotNull().WithMessage("CanManageReviews is required.");
        });
        When(x => x.CanManageTransactions != null, () => {
            RuleFor(x => x.CanManageTransactions)
                .NotNull().WithMessage("CanManageTransactions is required.");
        });
    }
}
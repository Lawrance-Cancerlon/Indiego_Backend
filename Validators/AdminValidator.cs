using System;
using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Validators;

public class CreateAdminValidator : AbstractValidator<CreateAdminContract>
{
    public CreateAdminValidator(IUserRepository<User> userRepository)
    {
        Include(new CreateUserValidator(userRepository));
        RuleFor(x => x.CanManageAdmins)
            .NotNull().WithMessage("CanManageAdmins is required.");
        RuleFor(x => x.CanManageGames)
            .NotNull().WithMessage("CanManageGames is required.");
        RuleFor(x => x.CanManagePosts)
            .NotNull().WithMessage("CanManagePosts is required.");
        RuleFor(x => x.CanManageReviews)
            .NotNull().WithMessage("CanManageReviews is required.");
        RuleFor(x => x.CanManageSubscriptions)
            .NotNull().WithMessage("CanManageTransactions is required.");
    }
}

public class UpdateAdminValidator : AbstractValidator<UpdateAdminContract>
{
    public UpdateAdminValidator(IUserRepository<User> userRepository)
    {
        Include(new UpdateUserValidator(userRepository));
        When(x => x.CanManageAdmins != null, () => {
            RuleFor(x => x.CanManageAdmins)
                .NotNull().WithMessage("CanManageAdmins is required.");
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
        When(x => x.CanManageSubscriptions != null, () => {
            RuleFor(x => x.CanManageSubscriptions)
                .NotNull().WithMessage("CanManageTransactions is required.");
        });
    }
}
using System;
using FluentValidation;
using Indiego_Backend.Contracts.Users;

namespace Indiego_Backend.Validations;

public class CreateAdminValidation : AbstractValidator<CreateAdminContract>
{
    public CreateAdminValidation()
    {
        Include(new CreateUserValidation());
        RuleFor(p => p.CanManageAdmins).NotNull().WithMessage("CanManageAdmins is required");
        RuleFor(p => p.CanManageUsers).NotNull().WithMessage("CanManageUsers is required");
        RuleFor(p => p.CanManageGames).NotNull().WithMessage("CanManageGames is required");
        RuleFor(p => p.CanManagePosts).NotNull().WithMessage("CanManagePosts is required");
        RuleFor(p => p.CanManageReviews).NotNull().WithMessage("CanManageReviews is required");
        RuleFor(p => p.CanManageSubscriptions).NotNull().WithMessage("CanManageSubscriptions is required");
        RuleFor(p => p.CanManageTransactions).NotNull().WithMessage("CanManageTransactions is required");
    }
}

public class UpdateAdminValidation : AbstractValidator<UpdateAdminContract>
{
    public UpdateAdminValidation()
    {
        Include(new UpdateUserValidation());
    }
}
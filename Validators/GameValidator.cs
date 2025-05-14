using System;
using FluentValidation;
using Indiego_Backend.Contracts;

namespace Indiego_Backend.Validators;

public class CreateGameValidator : AbstractValidator<CreateGameContract>
{
    public CreateGameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 20).WithMessage("Name must be between 3 and 20 characters long.")
            .Matches(@"^[a-zA-Z0-9 ]+$").WithMessage("Name can only contain letters, numbers, and spaces.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Length(10, 500).WithMessage("Description must be between 10 and 500 characters long.");

        RuleFor(x => x.GenreIds)
            .NotEmpty().WithMessage("At least one genre is required.");
    }
}

public class UpdateGameValidator : AbstractValidator<UpdateGameContract>
{
    public UpdateGameValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Name), () => {
            RuleFor(x => x.Name)
                .Length(3, 20).WithMessage("Name must be between 3 and 20 characters long.")
                .Matches(@"^[a-zA-Z0-9 ]+$").WithMessage("Name can only contain letters, numbers, and spaces.");
        });
        When(x => !string.IsNullOrEmpty(x.Description), () => {
            RuleFor(x => x.Description)
                .Length(10, 500).WithMessage("Description must be between 10 and 500 characters long.");
        });
        When(x => x.GenreIds != null && x.GenreIds.Count > 0, () => {
            RuleFor(x => x.GenreIds)
                .NotEmpty().WithMessage("At least one genre is required.");
        });
    }
}
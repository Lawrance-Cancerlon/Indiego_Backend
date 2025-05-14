using System;
using FluentValidation;
using Indiego_Backend.Contracts;

namespace Indiego_Backend.Validators;

public class CreateGenreValidator : AbstractValidator<CreateGenreContract>
{
    public CreateGenreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
    }
}

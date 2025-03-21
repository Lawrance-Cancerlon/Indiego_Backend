using System;
using FluentValidation;
using Indiego_Backend.Contracts.Games;

namespace Indiego_Backend.Validations;

public class CreateGenreValidation : AbstractValidator<CreateGenreContract>
{
    public CreateGenreValidation()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required");
    }
}

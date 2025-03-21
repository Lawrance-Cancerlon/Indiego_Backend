using System;
using FluentValidation;
using Indiego_Backend.Contracts.Games;

namespace Indiego_Backend.Validations;

public class CreateGameValidation : AbstractValidator<CreateGameContract>
{
    public CreateGameValidation()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(p => p.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(p => p.GenreIds).NotEmpty().WithMessage("GenreIds is required");
    }
}

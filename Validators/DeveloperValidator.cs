using System;
using FluentValidation;
using Indiego_Backend.Contracts;

namespace Indiego_Backend.Validators;

public class UpdateDeveloperValidator : AbstractValidator<UpdateDeveloperContract>
{
    public UpdateDeveloperValidator()
    {
        Include(new UpdateUserValidator());
        When(x => x.GameIds != null, () => {
            RuleFor(x => x.GameIds)
                .Must(x => x != null && x.All(id => Guid.TryParse(id, out _))).WithMessage("All GameIds must be valid GUIDs.");
        });
        When(x => x.PostIds != null, () => {
            RuleFor(x => x.PostIds)
                .Must(x => x != null && x.All(id => Guid.TryParse(id, out _))).WithMessage("All PostIds must be valid GUIDs.");
        });
    }
}

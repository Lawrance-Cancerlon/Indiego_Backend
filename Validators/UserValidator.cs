using System;
using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserContract>
{
    private readonly IUserRepository<User> _userRepository;

    public CreateUserValidator(IUserRepository<User> userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 100).WithMessage("Name must be between 3 and 100 characters long.")
            .Matches(@"^[a-zA-Z0-9 ]+$").WithMessage("Name can only contain letters, numbers, and spaces.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(8, 100).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .Must(email => !EmailExistsInDatabase(email)).WithMessage("Email is already registered.");
    }

    private bool EmailExistsInDatabase(string email)
    {
        return _userRepository.Get(null, email).Result.FirstOrDefault() != null;
    }
}

public class UpdateUserValidator : AbstractValidator<UpdateUserContract>
{
    private readonly IUserRepository<User> _userRepository;

    public UpdateUserValidator(IUserRepository<User> userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .Length(3, 100).WithMessage("Name must be between 3 and 100 characters long.")
                .Matches(@"^[a-zA-Z0-9 ]+$").WithMessage("Name can only contain letters, numbers, and spaces.");
        });
        When(x => !string.IsNullOrEmpty(x.Password), () =>
        {
            RuleFor(x => x.Password)
                .Length(8, 100).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
        });
        When(x => !string.IsNullOrEmpty(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .Must(email => !EmailExistsInDatabase(email)).WithMessage("Email is already registered.");
        });
    }

    private bool EmailExistsInDatabase(string email)
    {
        return _userRepository.Get(null, email).Result.FirstOrDefault() != null;
    }
}
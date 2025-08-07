using BidFlow.DTOs.User;
using FluentValidation;

namespace BidFlow.Validators.User
{
    public class CreateUserValidator : BaseValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .MaximumLength(100)
                .WithMessage("Username cannot exceed 100 characters")
                .Matches("^[a-zA-Z0-9_]+$")
                .WithMessage("Username can only contain letters, numbers and underscores");

            ValidateEmail(x => x.Email);

            ValidatePassword(x => x.Password);

            RuleFor(x => x.FirstName)
                .MaximumLength(50)
                .WithMessage("First name cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(50)
                .WithMessage("Last name cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.LastName));
        }
    }
}

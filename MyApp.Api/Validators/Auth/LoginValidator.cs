using BidFlow.DTOs.Auth;
using FluentValidation;

namespace BidFlow.Validators.Auth
{
    public class LoginValidator : BaseValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .MaximumLength(100)
                .WithMessage("Username cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");
        }
    }
}

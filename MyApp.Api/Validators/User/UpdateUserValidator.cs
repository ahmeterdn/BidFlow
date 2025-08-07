using BidFlow.DTOs.User;
using FluentValidation;

namespace BidFlow.Validators.User
{
    public class UpdateUserValidator : BaseValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            ValidateEmail(x => x.Email);

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
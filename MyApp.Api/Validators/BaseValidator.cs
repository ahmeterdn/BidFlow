using BidFlow.Common;
using System.Linq.Expressions;
using FluentValidation;

namespace BidFlow.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        protected void ValidateEmail(Expression<Func<T, string>> expression)
        {
            RuleFor(expression)
                .NotEmpty()
                .WithMessage("Email is required")
                .Must(email => IsValidEmail(email))
                .WithMessage(ErrorMessages.InvalidEmail);
        }

        protected void ValidatePassword(Expression<Func<T, string>> expression)
        {
            RuleFor(expression)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage(ErrorMessages.PasswordTooShort);
        }

        protected void ValidateRequired<TProperty>(Expression<Func<T, TProperty>> expression, string fieldName, int? maxLength = null)
        {
            var rule = RuleFor(expression)
                .NotEmpty()
                .WithMessage($"{fieldName} is required");

            if (maxLength.HasValue)
            {
                rule.Must(value => value?.ToString()?.Length <= maxLength.Value)
                    .WithMessage($"{fieldName} cannot exceed {maxLength.Value} characters");
            }
        }

        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

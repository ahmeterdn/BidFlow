using FluentValidation;

namespace BidFlow.Common
{
    public static class ValidationExtensions
    {
        public static async Task<Result<T>> ValidateAsync<T>(this IValidator<T> validator, T instance)
        {
            var validationResult = await validator.ValidateAsync(instance);

            if (validationResult.IsValid)
            {
                return Result<T>.Success(instance);
            }

            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Result<T>.Failure(ErrorMessages.ValidationFailed, errors);
        }

        public static Result<T> Validate<T>(this IValidator<T> validator, T instance)
        {
            var validationResult = validator.Validate(instance);

            if (validationResult.IsValid)
            {
                return Result<T>.Success(instance);
            }

            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Result<T>.Failure(ErrorMessages.ValidationFailed, errors);
        }
    }
}

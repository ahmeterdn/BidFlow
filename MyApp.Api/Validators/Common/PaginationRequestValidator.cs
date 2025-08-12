using BidFlow.DTOs.Common;
using FluentValidation;

namespace BidFlow.Validators.Common
{
    public class PaginationRequestValidator : AbstractValidator<PaginationRequestDto>
    {
        public PaginationRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0")
                .LessThanOrEqualTo(1000)
                .WithMessage("Page number cannot exceed 1000");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size cannot exceed 100");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));

            RuleFor(x => x.SortBy)
                .MaximumLength(50)
                .WithMessage("Sort field cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.SortBy));

            RuleFor(x => x.SortDirection)
                .Must(x => x == null || x.ToLower() == "asc" || x.ToLower() == "desc")
                .WithMessage("Sort direction must be 'asc' or 'desc'")
                .When(x => !string.IsNullOrEmpty(x.SortDirection));
        }
    }
}

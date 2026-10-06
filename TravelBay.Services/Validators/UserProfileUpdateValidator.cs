using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class UserProfileUpdateValidator : AbstractValidator<UserProfileUpdateRequest>
    {
        public UserProfileUpdateValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .Matches(PhoneNumberRules.Pattern).WithMessage(PhoneNumberRules.Message)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }

    /// <summary>Shared phone format: optional leading +, then 6–20 digits, spaces, slashes or dashes.</summary>
    public static class PhoneNumberRules
    {
        public const string Pattern = @"^\+?[0-9 /-]{6,20}$";
        public const string Message = "Phone number may contain only digits, spaces, '/', '-' and a leading '+' (6-20 characters).";
    }
}

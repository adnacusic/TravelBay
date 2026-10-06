using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    /// <summary>Same password rules as registration (UserInsertValidator).</summary>
    internal static class PasswordRules
    {
        public const int MinLength = 6;
        public const int MaxLength = 100;
    }

    public class UserPasswordChangeValidator : AbstractValidator<UserPasswordChangeRequest>
    {
        public UserPasswordChangeValidator()
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(PasswordRules.MinLength).WithMessage($"New password must be at least {PasswordRules.MinLength} characters.")
                .MaximumLength(PasswordRules.MaxLength).WithMessage($"New password cannot exceed {PasswordRules.MaxLength} characters.");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Password confirmation does not match the new password.");
        }
    }

    public class UserPasswordResetValidator : AbstractValidator<UserPasswordResetRequest>
    {
        public UserPasswordResetValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(PasswordRules.MinLength).WithMessage($"New password must be at least {PasswordRules.MinLength} characters.")
                .MaximumLength(PasswordRules.MaxLength).WithMessage($"New password cannot exceed {PasswordRules.MaxLength} characters.");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Password confirmation does not match the new password.");
        }
    }
}

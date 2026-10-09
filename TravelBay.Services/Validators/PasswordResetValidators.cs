using TravelBay.Model.Constants;
using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
        }
    }

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Matches($"^[0-9]{{{PasswordResetRules.CodeLength}}}$")
                .WithMessage($"The code has {PasswordResetRules.CodeLength} digits.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(PasswordRules.MinLength).WithMessage($"New password must be at least {PasswordRules.MinLength} characters.")
                .MaximumLength(PasswordRules.MaxLength).WithMessage($"New password cannot exceed {PasswordRules.MaxLength} characters.");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Password confirmation does not match the new password.");
        }
    }
}

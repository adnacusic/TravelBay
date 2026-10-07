using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class UserProfileImageValidator : AbstractValidator<UserProfileImageRequest>
    {
        public UserProfileImageValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .MaximumLength(ImageRules.MaxFileNameLength).WithMessage($"File name cannot exceed {ImageRules.MaxFileNameLength} characters.");

            RuleFor(x => x.ContentType)
                .Must(ImageRules.BeImageContentType).WithMessage("Only image files are allowed.");

            RuleFor(x => x.Base64Content)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Image is required.")
                .Must(ImageRules.BeValidImageSize).WithMessage($"Image must be valid base64 and at most {ImageRules.MaxImageBytes / (1024 * 1024)} MB.");
        }
    }
}

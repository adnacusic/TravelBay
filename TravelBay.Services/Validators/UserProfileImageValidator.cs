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
                .Must(ImageRules.BeAllowedContentType).WithMessage(ImageRules.AllowedTypesMessage);

            RuleFor(x => x.Base64Content)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Image is required.")
                .Must(ImageRules.BeValidImageSize).WithMessage(ImageRules.SizeMessage)
                .Must((request, base64) => ImageRules.HaveMatchingSignature(base64, request.ContentType))
                .WithMessage(ImageRules.SignatureMessage);
        }
    }
}

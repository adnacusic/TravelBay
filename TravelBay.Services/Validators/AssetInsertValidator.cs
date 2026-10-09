using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class AssetInsertValidator : AbstractValidator<AssetInsertRequest>
    {
        public AssetInsertValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("FileName is required.")
                .MaximumLength(100).WithMessage("FileName cannot exceed 100 characters.");

            // The asset store only holds images (profile pictures, uploaded destination/news photos).
            RuleFor(x => x.ContentType)
                .Must(ImageRules.BeAllowedContentType).WithMessage(ImageRules.AllowedTypesMessage);

            RuleFor(x => x.Base64Content)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Base64Content is required.")
                .Must(ImageRules.BeValidImageSize).WithMessage(ImageRules.SizeMessage)
                .Must((request, base64) => ImageRules.HaveMatchingSignature(base64, request.ContentType))
                .WithMessage(ImageRules.SignatureMessage);
        }
    }
}

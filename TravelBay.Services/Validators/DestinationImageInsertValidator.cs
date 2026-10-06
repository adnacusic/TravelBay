using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class DestinationImageInsertValidator : AbstractValidator<DestinationImageInsertRequest>
    {
        public DestinationImageInsertValidator()
        {
            RuleFor(x => x.DestinationId).GreaterThan(0).WithMessage("DestinationId is required.");

            RuleFor(x => x)
                .Must(x => string.IsNullOrWhiteSpace(x.ImageUrl) != string.IsNullOrWhiteSpace(x.Base64Content))
                .OverridePropertyName("Image")
                .WithMessage("Provide either an image URL or an uploaded image file, not both.");

            When(x => !string.IsNullOrWhiteSpace(x.ImageUrl), () =>
            {
                RuleFor(x => x.ImageUrl!)
                    .MaximumLength(ImageRules.MaxImageUrlLength).WithMessage($"Image URL cannot exceed {ImageRules.MaxImageUrlLength} characters.")
                    .Must(ImageRules.BeHttpUrl).WithMessage("Image URL must be an absolute http or https address.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Base64Content), () =>
            {
                RuleFor(x => x.FileName)
                    .NotEmpty().WithMessage("File name is required.")
                    .MaximumLength(ImageRules.MaxFileNameLength).WithMessage($"File name cannot exceed {ImageRules.MaxFileNameLength} characters.");

                RuleFor(x => x.ContentType)
                    .Must(ImageRules.BeImageContentType).WithMessage("Only image files are allowed.");

                RuleFor(x => x.Base64Content!)
                    .Must(ImageRules.BeValidImageSize).WithMessage($"Image must be valid base64 and at most {ImageRules.MaxImageBytes / (1024 * 1024)} MB.");
            });
        }
    }
}

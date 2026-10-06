using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    /// <summary>Rules for news create/update; only the insert requires an image.</summary>
    public abstract class NewsRequestValidator<T> : AbstractValidator<T> where T : NewsInsertRequest
    {
        public const int MaxTitleLength = 200;
        public const int MaxContentLength = 5000;

        protected NewsRequestValidator(bool imageRequired)
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(MaxTitleLength).WithMessage($"Title cannot exceed {MaxTitleLength} characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(MaxContentLength).WithMessage($"Content cannot exceed {MaxContentLength} characters.");

            RuleFor(x => x.PublishedAt)
                .NotEqual(default(DateTime)).WithMessage("Publication date is required.");

            RuleFor(x => x)
                .Must(x => !(HasUrl(x) && HasFile(x)))
                .OverridePropertyName("Image")
                .WithMessage("Provide either an image URL or an uploaded image file, not both.");

            if (imageRequired)
            {
                RuleFor(x => x)
                    .Must(x => HasUrl(x) || HasFile(x))
                    .OverridePropertyName("Image")
                    .WithMessage("An image is required.");
            }

            When(HasUrl, () =>
            {
                RuleFor(x => x.ImageUrl!)
                    .MaximumLength(ImageRules.MaxImageUrlLength).WithMessage($"Image URL cannot exceed {ImageRules.MaxImageUrlLength} characters.")
                    .Must(ImageRules.BeHttpUrl).WithMessage("Image URL must be an absolute http or https address.");
            });

            When(HasFile, () =>
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

        private static bool HasUrl(T request) => !string.IsNullOrWhiteSpace(request.ImageUrl);
        private static bool HasFile(T request) => !string.IsNullOrWhiteSpace(request.Base64Content);
    }

    public class NewsInsertValidator : NewsRequestValidator<NewsInsertRequest>
    {
        public NewsInsertValidator() : base(imageRequired: true)
        {
        }
    }
}

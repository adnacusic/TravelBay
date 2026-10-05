using TravelBay.Model.Requests;
using FluentValidation;

namespace TravelBay.Services.Validators
{
    public class DestinationImageInsertValidator : AbstractValidator<DestinationImageInsertRequest>
    {
        private const int MaxImageUrlLength = 500;
        private const int MaxFileNameLength = 100;
        private const int MaxImageBytes = 5 * 1024 * 1024;
        private const string ImageContentTypePrefix = "image/";

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
                    .MaximumLength(MaxImageUrlLength).WithMessage($"Image URL cannot exceed {MaxImageUrlLength} characters.")
                    .Must(BeHttpUrl).WithMessage("Image URL must be an absolute http or https address.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Base64Content), () =>
            {
                RuleFor(x => x.FileName)
                    .NotEmpty().WithMessage("File name is required.")
                    .MaximumLength(MaxFileNameLength).WithMessage($"File name cannot exceed {MaxFileNameLength} characters.");

                RuleFor(x => x.ContentType)
                    .NotEmpty().WithMessage("Content type is required.")
                    .Must(c => c != null && c.StartsWith(ImageContentTypePrefix, StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Only image files are allowed.");

                RuleFor(x => x.Base64Content!)
                    .Must(BeValidImageSize).WithMessage($"Image must be valid base64 and at most {MaxImageBytes / (1024 * 1024)} MB.");
            });
        }

        private static bool BeHttpUrl(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        private static bool BeValidImageSize(string base64)
        {
            var buffer = new byte[base64.Length];
            return Convert.TryFromBase64String(base64, buffer, out var bytesWritten)
                && bytesWritten > 0
                && bytesWritten <= MaxImageBytes;
        }
    }
}

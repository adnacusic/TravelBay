namespace TravelBay.Services.Validators
{
    /// <summary>Rules shared by every request that carries an image as a URL or as an uploaded file.</summary>
    public static class ImageRules
    {
        public const int MaxImageUrlLength = 500;
        public const int MaxFileNameLength = 100;
        public const int MaxImageBytes = 5 * 1024 * 1024;
        public const string ImageContentTypePrefix = "image/";

        public static bool BeHttpUrl(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        public static bool BeImageContentType(string? contentType) =>
            contentType != null && contentType.StartsWith(ImageContentTypePrefix, StringComparison.OrdinalIgnoreCase);

        public static bool BeValidImageSize(string base64)
        {
            var buffer = new byte[base64.Length];
            return Convert.TryFromBase64String(base64, buffer, out var bytesWritten)
                && bytesWritten > 0
                && bytesWritten <= MaxImageBytes;
        }
    }
}

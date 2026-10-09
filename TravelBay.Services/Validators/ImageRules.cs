namespace TravelBay.Services.Validators
{
    /// <summary>Rules shared by every request that carries an image as a URL or as an uploaded file.</summary>
    public static class ImageRules
    {
        public const int MaxImageUrlLength = 500;
        public const int MaxFileNameLength = 100;
        public const int MaxImageBytes = 5 * 1024 * 1024;

        public const string JpegContentType = "image/jpeg";
        public const string PngContentType = "image/png";
        public const string WebpContentType = "image/webp";

        /// <summary>Raster photos only: SVG (can carry scripts), GIF and BMP are refused.</summary>
        public static readonly IReadOnlySet<string> AllowedContentTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { JpegContentType, PngContentType, WebpContentType };

        public const string AllowedTypesMessage = "Only JPEG, PNG or WebP images are allowed.";
        public const string SignatureMessage = "The file is not a JPEG, PNG or WebP image, or its content does not match its type.";
        public static readonly string SizeMessage = $"Image must be valid base64 and at most {MaxImageBytes / (1024 * 1024)} MB.";

        private static readonly byte[] JpegSignature = { 0xFF, 0xD8, 0xFF };
        private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private static readonly byte[] RiffSignature = "RIFF"u8.ToArray();
        private static readonly byte[] WebpSignature = "WEBP"u8.ToArray();
        private const int WebpSignatureOffset = 8;

        public static bool BeHttpUrl(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        public static bool BeAllowedContentType(string? contentType) =>
            contentType != null && AllowedContentTypes.Contains(contentType);

        public static bool BeValidImageSize(string base64) => TryDecode(base64, out _);

        /// <summary>
        /// The bytes themselves must be a JPEG, PNG or WebP of the declared type (magic bytes), so a
        /// renamed script or SVG with an "image/png" content type is refused.
        /// </summary>
        public static bool HaveMatchingSignature(string base64, string? contentType) =>
            TryDecode(base64, out var bytes)
            && DetectContentType(bytes) is { } detected
            && string.Equals(detected, contentType, StringComparison.OrdinalIgnoreCase);

        public static string? DetectContentType(ReadOnlySpan<byte> bytes)
        {
            if (bytes.StartsWith(JpegSignature))
            {
                return JpegContentType;
            }
            if (bytes.StartsWith(PngSignature))
            {
                return PngContentType;
            }
            if (bytes.StartsWith(RiffSignature)
                && bytes.Length >= WebpSignatureOffset + WebpSignature.Length
                && bytes.Slice(WebpSignatureOffset, WebpSignature.Length).SequenceEqual(WebpSignature))
            {
                return WebpContentType;
            }
            return null;
        }

        private static bool TryDecode(string base64, out byte[] bytes)
        {
            var buffer = new byte[base64.Length];
            if (Convert.TryFromBase64String(base64, buffer, out var written) && written > 0 && written <= MaxImageBytes)
            {
                bytes = buffer[..written];
                return true;
            }
            bytes = Array.Empty<byte>();
            return false;
        }
    }
}

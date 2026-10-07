namespace TravelBay.Model.Requests
{
    /// <summary>New profile picture for the signed-in user (Users/Me/ProfileImage), sent as base64.</summary>
    public class UserProfileImageRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Base64Content { get; set; } = string.Empty;
    }
}

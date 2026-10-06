namespace TravelBay.Model.Requests
{
    /// <summary>Own-profile edit (Users/Me). Account status and role are never part of it.</summary>
    public class UserProfileUpdateRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}

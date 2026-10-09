namespace TravelBay.Model.Requests
{
    /// <summary>
    /// Self-registration (Access/Register). Only personal data and a password: the account is always
    /// active, gets the User role, and has no picture until the user uploads one.
    /// </summary>
    public class UserRegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}

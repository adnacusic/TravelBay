namespace TravelBay.Model.Requests
{
    /// <summary>Current-user self-service password change — the target user always comes from the JWT, never from this body.</summary>
    public class UserPasswordChangeRequest
    {
        public string Password { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
    }
}

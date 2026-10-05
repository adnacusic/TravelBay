namespace TravelBay.Model.Requests
{
    /// <summary>Admin resetting another user's password — no old password required.</summary>
    public class UserPasswordResetRequest
    {
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
    }
}

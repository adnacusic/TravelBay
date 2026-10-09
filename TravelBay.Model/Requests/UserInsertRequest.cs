namespace TravelBay.Model.Requests
{
    /// <summary>Admin creating an account: the registration data plus whether it starts active.</summary>
    public class UserInsertRequest : UserRegisterRequest
    {
        public bool IsActive { get; set; } = true;
    }
}

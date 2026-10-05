namespace TravelBay.Model.Constants
{
    /// <summary>JWT claim type names, shared by token issuing (AccessManager), reading (accessor/hub) and authorization.</summary>
    public static class ClaimNames
    {
        public const string Id = "Id";
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string Email = "Email";
        public const string Role = "Role";
        public const string IsActive = "IsActive";
    }
}

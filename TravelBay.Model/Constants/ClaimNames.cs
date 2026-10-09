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

        /// <summary>Standard JWT ids (RFC 7519): unique token id, used to revoke a token on logout, and expiry.</summary>
        public const string TokenId = "jti";
        public const string ExpiresAt = "exp";
    }
}

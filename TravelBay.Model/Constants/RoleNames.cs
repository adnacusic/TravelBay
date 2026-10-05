namespace TravelBay.Model.Constants
{
    /// <summary>Canonical role names, used by both the seed and [Authorize(Roles=...)] checks so they can never drift apart.</summary>
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}

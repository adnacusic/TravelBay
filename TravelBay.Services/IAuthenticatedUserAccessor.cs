namespace TravelBay.Services;

/// <summary>
/// Provides the authenticated application user for the current request (when available).
/// </summary>
public interface IAuthenticatedUserAccessor
{
    int? GetUserId();

    /// <summary>
    /// Returns whether the current principal is authenticated and has the given application role (JWT Role claim).
    /// </summary>
    bool IsInRole(string role);

    /// <summary>The jti of the access token used for this request (needed to revoke it on logout).</summary>
    string? GetTokenId();

    /// <summary>When the access token used for this request expires (UTC).</summary>
    DateTime? GetTokenExpiresAt();
}

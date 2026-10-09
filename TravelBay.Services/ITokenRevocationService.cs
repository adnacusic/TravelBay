namespace TravelBay.Services;

/// <summary>Logout: refresh tokens are deleted and the current access token is blacklisted until it expires.</summary>
public interface ITokenRevocationService
{
    Task RevokeAsync(int userId, string jti, DateTime expiresAt);

    Task<bool> IsRevokedAsync(string jti);
}

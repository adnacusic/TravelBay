using TravelBay.Model.Access;

namespace TravelBay.WebAPI.Services.AccessManager
{
    public interface IAccessManager
    {
        Task<UserLoginResponse> LoginAsync(UserLoginRequest request);
        Task<UserLoginResponse> LoginWithRefreshTokenAsync(RefreshAccessTokenRequest request);

        /// <summary>Signs the current user out: refresh tokens deleted, current access token revoked.</summary>
        Task LogoutAsync();
    }
}

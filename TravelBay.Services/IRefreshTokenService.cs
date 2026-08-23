using TravelBay.Services.Database;


namespace TravelBay.Services
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GetStoredTokenAsync(string refreshToken);
        Task InsertAsync(RefreshToken refreshToken);
        Task DeleteAllUserRefreshTokensAsync(int userId);
    }
}
